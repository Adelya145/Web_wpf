<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'
import { useRouter } from 'vue-router'

const { cookies } = useCookies()
const router = useRouter()

const basketItems = ref([])
const orders = ref([])
const pickUpPoints = ref([])
const loading = ref(false)
const errorMessage = ref('')
const showOrders = ref(false)
const showCheckoutModal = ref(false)
const selectedPickUpPoint = ref(null)

// Уведомления
const notification = ref({
  show: false,
  type: '',
  message: ''
})

const userData = computed(() => {
  return cookies.get('user')
})

const isAuthenticatedUser = computed(() => {
  const user = userData.value
  return user && user.userRole === 'Авторизированный клиент'
})

// Проверка авторизации при загрузке
if (!isAuthenticatedUser.value) {
  router.push('/')
}

const showNotification = (type, message) => {
  notification.value = {
    show: true,
    type,
    message
  }
  
  setTimeout(() => {
    notification.value.show = false
  }, 3000)
}

// Безопасное получение числа
const safeNumber = (value, defaultValue = 0) => {
  if (value === null || value === undefined) return defaultValue
  const num = Number(value)
  return isNaN(num) ? defaultValue : num
}

// Безопасное форматирование цены
const formatPrice = (price) => {
  const num = safeNumber(price)
  return num.toLocaleString('ru-RU')
}

// Форматирование даты
const formatDate = (dateString) => {
  if (!dateString) return 'Н/Д'
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU')
}

const fetchBasketItems = async () => {
  if (!userData.value) return
  
  loading.value = true
  errorMessage.value = ''
  
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetBasketItems', {
      params: { userId: userData.value.userId }
    })
    
    console.log('Ответ от сервера:', response.data)
    
    basketItems.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки корзины:', error)
    errorMessage.value = 'Ошибка загрузки корзины'
    showNotification('error', 'Ошибка загрузки корзины')
  } finally {
    loading.value = false
  }
}

const fetchUserOrders = async () => {
  if (!userData.value) return
  
  loading.value = true
  
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetUserOrders', {
      params: { userId: userData.value.userId }
    })
    
    orders.value = response.data
    showOrders.value = true
  } catch (error) {
    console.error('Ошибка загрузки заказов:', error)
    showNotification('error', 'Ошибка загрузки заказов')
  } finally {
    loading.value = false
  }
}

const fetchPickUpPoints = async () => {
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetPickUpPoints')
    pickUpPoints.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки пунктов выдачи:', error)
  }
}

const updateQuantity = async (item, newQuantity) => {
  if (newQuantity < 1) {
    removeItem(item)
    return
  }

  const maxQty = safeNumber(item.maxQuantity)
  if (newQuantity > maxQty) {
    showNotification('error', `Нельзя добавить больше ${maxQty} шт. (доступно на складе)`)
    return
  }

  try {
    await axios.put('http://localhost:5166/api/Tovar/UpdateBasketQuantity', {
      userId: userData.value.userId,
      tovarArticle: item.tovarArticle,
      quantity: newQuantity
    })
    
    item.quantity = newQuantity
    showNotification('success', 'Количество обновлено')
  } catch (error) {
    const errorMsg = error.response?.data || 'Ошибка обновления количества'
    showNotification('error', errorMsg)
  }
}

const removeItem = async (item) => {
  if (!confirm('Удалить товар из корзины?')) return

  try {
    await axios.delete('http://localhost:5166/api/Tovar/RemoveFromBasket', {
      data: {
        userId: userData.value.userId,
        tovarArticle: item.tovarArticle
      }
    })
    
    basketItems.value = basketItems.value.filter(i => i.tovarArticle !== item.tovarArticle)
    showNotification('success', 'Товар удален из корзины')
  } catch (error) {
    showNotification('error', 'Ошибка удаления товара')
  }
}

const clearBasket = async () => {
  if (!confirm('Очистить корзину?')) return

  try {
    await axios.delete(`http://localhost:5166/api/Tovar/ClearBasket/${userData.value.userId}`)
    
    basketItems.value = []
    showNotification('success', 'Корзина очищена')
  } catch (error) {
    showNotification('error', 'Ошибка очистки корзины')
  }
}

// Вычисление цены со скидкой
const getDiscountedPrice = (item) => {
  const price = safeNumber(item.product?.tovarCost)
  const discount = safeNumber(item.product?.tovarCurrentSale)
  
  if (discount > 0) {
    return Math.round(price * (1 - discount / 100))
  }
  return price
}

// Вычисление суммы для одного товара
const getItemTotal = (item) => {
  const price = getDiscountedPrice(item)
  const quantity = safeNumber(item.quantity)
  return price * quantity
}

// Общая сумма
const totalSum = computed(() => {
  return basketItems.value.reduce((sum, item) => {
    return sum + getItemTotal(item)
  }, 0)
})

// Общее количество товаров
const totalItems = computed(() => {
  return basketItems.value.reduce((sum, item) => {
    return sum + safeNumber(item.quantity)
  }, 0)
})

const continueShopping = () => {
  router.push('/home')
}

const openCheckoutModal = async () => {
  await fetchPickUpPoints()
  showCheckoutModal.value = true
}

const closeCheckoutModal = () => {
  showCheckoutModal.value = false
  selectedPickUpPoint.value = null
}

const createOrder = async () => {
  if (!selectedPickUpPoint.value) {
    showNotification('error', 'Выберите пункт выдачи')
    return
  }

  const orderItems = basketItems.value.map(item => ({
    tovarArticle: item.tovarArticle,
    quantity: item.quantity
  }))

  try {
    const response = await axios.post('http://localhost:5166/api/Tovar/CreateOrder', {
      userId: userData.value.userId,
      pickUpPointId: selectedPickUpPoint.value,
      items: orderItems
    })

    if (response.data.success) {
      showNotification('success', `Заказ №${response.data.orderCode} успешно оформлен`)
      closeCheckoutModal()
      await fetchBasketItems() // Обновляем корзину
    }
  } catch (error) {
    const errorMsg = error.response?.data || 'Ошибка оформления заказа'
    showNotification('error', errorMsg)
  }
}

const toggleOrders = () => {
  if (!showOrders.value) {
    fetchUserOrders()
  } else {
    showOrders.value = false
  }
}

onMounted(() => {
  fetchBasketItems()
})
</script>

<template>
  <div class="basket-page">
    <!-- Уведомления -->
    <transition name="notification">
      <div v-if="notification.show" :class="['notification-toast', notification.type]">
        <div class="notification-icon">
          <span v-if="notification.type === 'success'">✓</span>
          <span v-if="notification.type === 'error'">✗</span>
          <span v-if="notification.type === 'info'">ℹ</span>
        </div>
        <div class="notification-content">
          {{ notification.message }}
        </div>
      </div>
    </transition>

    <div class="basket-header">
      <h1>Корзина</h1>
      <div class="header-buttons">
        <button @click="toggleOrders" class="orders-btn">
          {{ showOrders ? 'Скрыть заказы' : 'Мои заказы' }}
        </button>
        <button @click="continueShopping" class="continue-btn">
          ← Продолжить покупки
        </button>
      </div>
    </div>

    <!-- Блок с заказами -->
    <div v-if="showOrders" class="orders-section">
      <h2>Мои заказы</h2>
      <span>Всего заказов: {{ orders.length }}</span>
      <div v-if="loading" class="loading">Загрузка заказов...</div>
      <div v-else-if="orders.length === 0" class="no-orders">
        <p>У вас пока нет заказов</p>
      </div>
      <div v-else class="orders-list">
        <div v-for="order in orders" :key="order.orderId" class="order-card">
          <div class="order-header">
            <div class="order-info">
                 <span class="order-number">Заказ №{{ order.orderCode }}</span>
                 
             

              <span class="order-date">от {{ formatDate(order.orderDate) }}</span>
              <span class="order-status" :class="order.orderStatus.toLowerCase()">{{ order.orderStatus }}</span>
            </div>
            <div class="order-delivery">
              Доставка ожидается: {{ formatDate(order.orderDateDelivery) }}
              <span class="pickup-point">{{ order.pickUpPointAddress }}</span>
            </div>
          </div>
          
          <div class="order-items">
            <div v-for="item in order.items" :key="item.orderCompositionId" class="order-item">
              <span class="item-name">{{ item.product?.tovarName }}</span>
              <span class="item-quantity">{{ item.orderCompositionCount }} шт.</span>
              <span class="item-price">{{ formatPrice(item.totalPrice) }}₽</span>
            </div>
          </div>
          
          <div class="order-footer">
            <span class="order-total">Итого: {{ formatPrice(order.totalSum) }}₽</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно оформления заказа -->
    <div v-if="showCheckoutModal" class="modal-overlay" @click.self="closeCheckoutModal">
      <div class="modal-content">
        <div class="modal-header">
          <h3>Оформление заказа</h3>
          <button class="close-btn" @click="closeCheckoutModal">×</button>
        </div>
        
        <div class="modal-body">
          <div class="order-summary">
            <h4>Ваш заказ:</h4>
            <div v-for="item in basketItems" :key="item.tovarArticle" class="order-summary-item">
              <span>{{ item.product?.tovarName }} x{{ item.quantity }}</span>
              <span>{{ formatPrice(getItemTotal(item)) }}₽</span>
            </div>
            <div class="order-total">
              <strong>Итого:</strong>
              <strong>{{ formatPrice(totalSum) }}₽</strong>
            </div>
          </div>

          <div class="pickup-point-select">
            <h4>Выберите пункт выдачи:</h4>
            <select v-model="selectedPickUpPoint" class="pickup-select">
              <option value="">-- Выберите пункт выдачи --</option>
              <option v-for="point in pickUpPoints" :key="point.pickUpPointId" :value="point.pickUpPointId">
                {{ point.fullAddress }}
              </option>
            </select>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeCheckoutModal" class="cancel-btn">Отмена</button>
          <button @click="createOrder" class="confirm-btn">Подтвердить заказ</button>
        </div>
      </div>
    </div>

    <div v-if="loading && !showOrders" class="loading">
      Загрузка корзины...
    </div>

    <div v-else-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>

    <div v-else-if="basketItems.length === 0" class="empty-basket">
      <svg class="empty-icon" xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
        <circle cx="9" cy="21" r="1"></circle>
        <circle cx="20" cy="21" r="1"></circle>
        <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
      </svg>
      <h2>Корзина пуста</h2>
      <p>Добавьте товары в корзину из каталога</p>
      <button @click="continueShopping" class="to-catalog-btn">
        Перейти в каталог
      </button>
    </div>

    <div v-else class="basket-content">
      <!-- Список товаров -->
      <div class="basket-items">
        <div v-for="item in basketItems" :key="item.tovarArticle" class="basket-item">
          <div class="item-image">
            <img 
              :src="`/resources/${item.product?.tovarPhoto || 'picture.png'}`" 
              :alt="item.product?.tovarName"
              @error="(e) => { e.target.src = '/resources/picture.png' }"
            >
          </div>
          
          <div class="item-info">
            <div class="item-name">{{ item.product?.tovarName || 'Без названия' }}</div>
            <div class="item-category">{{ item.product?.tovarCategoryName || 'Без категории' }}</div>
            <div class="item-manufacturer">{{ item.product?.manufacturerName || 'Неизвестный производитель' }}</div>
            <div class="item-article">Артикул: {{ item.tovarArticle || 'Н/Д' }}</div>
            <div class="price-class">
              <span class="original-price">{{ formatPrice(item.product.tovarCost) }}₽</span>
              <span>→</span>
              <span class="discounted-price">
                {{ formatPrice(getDiscountedPrice(item)) }}₽
              </span>
              <template v-if="item.product?.tovarCurrentSale > 0">
                <span class="discount-badge">-{{ item.product.tovarCurrentSale }}%</span>
              </template>
              <template v-else>
                <span class="price">{{ formatPrice(item.product?.tovarCost) }}₽</span>
              </template>
            </div>
            
          </div>
          
          
          <div class="item-quantity">
            <button @click="updateQuantity(item, item.quantity - 1)" class="quantity-btn">−</button>
            <span class="quantity">{{ item.quantity || 0 }}</span>
            <button @click="updateQuantity(item, item.quantity + 1)" class="quantity-btn">+</button>
            <span class="stock">(доступно: {{ item.maxQuantity || 0 }})</span>
          </div>
          
          <div class="item-total">
            <span class="total-label">Сумма:</span>
            <span class="total-value">{{ formatPrice(getItemTotal(item)) }}₽</span>
          </div>
          
          <button @click="removeItem(item)" class="remove-btn" title="Удалить товар">
            <span class="remove-icon">×</span>
          </button>
        </div>
      </div>

      <!-- Итого и действия -->
      <div class="basket-summary">
        <h3>Итого</h3>
        
        <div class="summary-details">
          <div class="summary-row">
            <span>Товаров в корзине:</span>
            <span>{{ totalItems }} шт.</span>
          </div>
          <div class="summary-row">
            <span>Общая сумма:</span>
            <span class="summary-total">{{ formatPrice(totalSum) }}₽</span>
          </div>
          <div v-if="basketItems.some(item => item.product?.tovarCurrentSale > 0)" class="summary-note">
            * Цены указаны с учетом скидок
          </div>
        </div>

        <div class="summary-actions">
          <button @click="clearBasket" class="clear-btn">
            Очистить корзину
          </button>
          <button @click="openCheckoutModal" class="checkout-btn">
            Оформить заказ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.notification-toast {
  position: fixed;
  top: 100px;
  right: 30px;
  min-width: 300px;
  max-width: 400px;
  background: white;
  border-radius: 8px;
  padding: 16px 20px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.2);
  display: flex;
  align-items: center;
  gap: 15px;
  z-index: 10000;
  border-left: 4px solid;
  animation: slideIn 0.3s ease-out;
}

.notification-toast.success {
  border-left-color: #28a745;
  background: #f0fff4;
}

.notification-toast.error {
  border-left-color: #dc3545;
  background: #fff5f5;
}

.notification-toast.info {
  border-left-color: #17a2b8;
  background: #e3f2fd;
}

.notification-icon {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1rem;
  font-weight: bold;
  flex-shrink: 0;
}

.success .notification-icon {
  background: #28a745;
  color: white;
}

.error .notification-icon {
  background: #dc3545;
  color: white;
}

.info .notification-icon {
  background: #17a2b8;
  color: white;
}

.notification-content {
  flex: 1;
  color: #333;
  font-size: 14px;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from,
.notification-leave-to {
  transform: translateX(100%);
  opacity: 0;
}

/* Шапка */
.basket-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #e6d9f2;
}

.basket-header h1 {
  font-size: 32px;
  font-weight: bold;
  color: #5d4a7a;
  margin: 0;
}

.header-buttons {
  display: flex;
  gap: 15px;
}

.orders-btn {
  padding: 10px 20px;
  background: #967bb6;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.orders-btn:hover {
  background: #7d63a0;
  transform: translateY(-2px);
}

.continue-btn {
  padding: 10px 20px;
  background: #f0f0f0;
  color: #666;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 5px;
}

.continue-btn:hover {
  background: #e0e0e0;
  color: #333;
  transform: translateX(-5px);
}

/* Секция заказов */
.orders-section {
  margin-bottom: 40px;
  padding: 20px;
  background: #f8f5ff;
  border-radius: 12px;
  border: 1px solid #e6d9f2;
}

.orders-section h2 {
  color: #5d4a7a;
  margin-bottom: 20px;
  font-size: 24px;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.order-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  border: 1px solid #e6d9f2;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 15px;
  border-bottom: 1px solid #e6d9f2;
  margin-bottom: 15px;
  flex-wrap: wrap;
  gap: 10px;
}

.order-info {
  display: flex;
  align-items: center;
  gap: 15px;
  flex-wrap: wrap;
}

.order-number {
  font-weight: 700;
  color: #5d4a7a;
  font-size: 16px;
}

.order-date {
  color: #666;
  font-size: 14px;
}

.order-status {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}
.price-class{
  display: flex;
  flex-direction: row;
  column-gap: 5px;
}

.order-status.оформлен {
  background: #e3f2fd;
  color: #1976d2;
}

.order-status.завершен {
  background: #e8f5e9;
  color: #388e3c;
}

.order-delivery {
  color: #666;
  font-size: 14px;
  text-align: right;
}

.pickup-point {
  display: block;
  color: #5d4a7a;
  font-size: 13px;
  margin-top: 5px;
}

.order-items {
  margin-bottom: 15px;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px dashed #f0f0f0;
}

.item-name {
  flex: 2;
  color: #333;
}

.item-quantity {
  flex: 1;
  text-align: center;
  color: #666;
}

.item-price {
  text-align: right;
  color: #5d4a7a;
  font-weight: 600;
}

.order-footer {
  display: flex;
  justify-content: flex-end;
  padding-top: 15px;
  border-top: 1px solid #e6d9f2;
}

.order-total {
  font-size: 16px;
  font-weight: 700;
  color: #b8860b;
}

.no-orders {
  text-align: center;
  padding: 40px;
  color: #666;
}

/* Модальное окно */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 11000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #e6d9f2;
}

.modal-header h3 {
  margin: 0;
  color: #5d4a7a;
  font-size: 20px;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  color: #666;
}

.modal-body {
  padding: 20px;
}

.order-summary {
  margin-bottom: 20px;
  padding: 15px;
  background: #f8f5ff;
  border-radius: 8px;
}

.order-summary h4 {
  margin: 0 0 15px 0;
  color: #5d4a7a;
}

.order-summary-item {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px dashed #e6d9f2;
}

.order-total {
  display: flex;
  justify-content: space-between;
  margin-top: 15px;
  padding-top: 15px;
  border-top: 2px solid #e6d9f2;
  color: #b8860b;
  font-size: 16px;
}

.pickup-point-select {
  margin-bottom: 20px;
}

.pickup-point-select h4 {
  margin: 0 0 10px 0;
  color: #5d4a7a;
}

.pickup-select {
  width: 100%;
  padding: 12px;
  border: 1px solid #d1c4e9;
  border-radius: 6px;
  font-size: 14px;
  background: white;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 15px;
  padding: 20px;
  border-top: 1px solid #e6d9f2;
}

.cancel-btn {
  padding: 10px 20px;
  background: #f0f0f0;
  color: #666;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.confirm-btn {
  padding: 10px 20px;
  background: #81694b;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.confirm-btn:hover {
  background: #6d5a3e;
}

/* Загрузка и ошибки */
.loading,
.error-message {
  text-align: center;
  padding: 60px;
  font-size: 18px;
  color: #666;
  background: #f9f9f9;
  border-radius: 8px;
}

.error-message {
  color: #dc3545;
  background: #fff5f5;
  border: 1px solid #ffcdd2;
}

/* Пустая корзина */
.empty-basket {
  text-align: center;
  padding: 60px;
  background: #f9f9f9;
  border-radius: 12px;
}

.empty-icon {
  color: #967bb6;
  opacity: 0.5;
  margin-bottom: 20px;
}

.empty-basket h2 {
  font-size: 28px;
  color: #333;
  margin-bottom: 10px;
}

.empty-basket p {
  color: #666;
  margin-bottom: 30px;
  font-size: 16px;
}

.to-catalog-btn {
  padding: 12px 30px;
  background: linear-gradient(135deg, #967bb6 0%, #b8a9d9 100%);
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.to-catalog-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(150, 123, 182, 0.3);
}

/* Контент корзины */
.basket-content {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 30px;
}

/* Список товаров */
.basket-items {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.basket-item {
  display: grid;
  grid-template-columns: 100px 1fr 200px 120px 40px;
  gap: 15px;
  align-items: center;
  padding: 20px;
  background: white;
  border-radius: 8px;
  border: 1px solid #e6d9f2;
  transition: all 0.3s;
}

.basket-item:hover {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  border-color: #967bb6;
}

.item-image {
  width: 100px;
  height: 100px;
  border-radius: 6px;
  overflow: hidden;
  background: #f5f0fa;
  border: 1px solid #e6d9f2;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  padding: 5px;
}

.item-info {
  display: flex;
  flex-direction: column;
  min-width: 200px;
  gap: 5px;
}

.item-name {
  font-weight: 600;
  color: #333;
  font-size: 16px;
}

.item-category {
  font-size: 13px;
  color: #967bb6;
  font-weight: 500;
}

.item-manufacturer {
  font-size: 13px;
  color: #666;
}

.item-article {
  font-size: 12px;
  color: #999;
  font-family: monospace;
  margin-top: 5px;
}

.item-price {
  display: grid;
  grid-template-columns: auto auto auto;
  gap: 5px;
  position: relative;
}

.original-price {
  font-size: 14px;
  color: #999;
  text-decoration: line-through;
}

.discounted-price {
  font-weight: 700;
  color: #b8860b;
  font-size: 14px;
}

.price {
  font-weight: 700;
  color: #5d4a7a;
  font-size: 18px;
}

.discount-badge {
  top: -5px;
  right: -5px;
  background: #dc3545;
  color: white;
  font-size: 12px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 4px;
}

.item-quantity {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  flex-wrap: wrap;
}

.quantity-btn {
  width: 32px;
  height: 32px;
  border: 1px solid #d1c4e9;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 18px;
  font-weight: bold;
  color: #5d4a7a;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: auto;
  padding: 0;
  transition: all 0.2s;
}

.quantity-btn:hover:not(:disabled) {
  background: #967bb6;
  color: white;
  border-color: #967bb6;
}

.quantity-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity {
  font-weight: 600;
  color: #333;
  min-width: 40px;
  text-align: center;
  font-size: 16px;
}

.stock {
  font-size: 12px;
  color: #666;
  margin-left: 5px;
}

.item-total {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

.total-label {
  font-size: 12px;
  color: #666;
  margin-bottom: 3px;
}

.total-value {
  font-weight: 700;
  color: #5d4a7a;
  font-size: 16px;
  white-space: nowrap;
}

.remove-btn {
  width: 32px;
  height: 32px;
  border: none;
  background: #f8d7da;
  color: #dc3545;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s;
  min-width: auto;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-btn:hover {
  background: #dc3545;
  color: white;
  transform: scale(1.1);
}

.remove-icon {
  font-size: 20px;
  line-height: 1;
}

/* Итого */
.basket-summary {
  background: white;
  border-radius: 8px;
  border: 1px solid #e6d9f2;
  padding: 25px;
  position: sticky;
  top: 120px;
  height: fit-content;
}

.basket-summary h3 {
  font-size: 20px;
  color: #5d4a7a;
  margin: 0 0 20px 0;
  padding-bottom: 15px;
  border-bottom: 1px solid #e6d9f2;
}

.summary-details {
  margin-bottom: 25px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 10px 0;
  color: #333;
  font-size: 15px;
}

.summary-row:not(:last-child) {
  border-bottom: 1px dashed #f0f0f0;
}

.summary-total {
  font-weight: 700;
  color: #b8860b;
  font-size: 20px;
}

.summary-note {
  margin-top: 15px;
  padding: 10px;
  background: #f8f5ff;
  border-radius: 4px;
  font-size: 13px;
  color: #666;
  font-style: italic;
  border-left: 3px solid #967bb6;
}

.summary-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.clear-btn {
  padding: 12px;
  background: #f0f0f0;
  color: #666;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.clear-btn:hover {
  background: #e0e0e0;
  color: #dc3545;
}

.checkout-btn {
  padding: 15px;
  background: #81694b;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.checkout-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px #766045;
}
</style>