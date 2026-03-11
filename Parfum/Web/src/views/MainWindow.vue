<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
import cardItem from '@/components/goodCard.vue'
import { useCookies } from 'vue3-cookies'
import AddEditTovarModal from '@/components/AddEditTovarModal.vue'
import { useRouter } from 'vue-router'

const { cookies } = useCookies()
const router = useRouter()

const products = ref([])
const suppliers = ref([])
const manufacturers = ref([])
const categories = ref([])

const searchQuery = ref('')
const selectedSupplier = ref('all')
const selectedManufacturer = ref('all')
const selectedCategory = ref('all')
const sortOption = ref('default')

const showModal = ref(false)
const editingProduct = ref(null)
const modalMode = ref('add')

const notification = ref({
  show: false,
  type: '',
  message: ''
})

const userRole = computed(() => {
  const user = cookies.get('user')
  return user?.userRole || null
})

const userData = computed(() => {
  return cookies.get('user')
})

const isAdmin = computed(() => {
  return userRole.value === 'Администратор'
})

const isManager = computed(() => {
  return userRole.value === 'Менеджер'
})

const isManagerOrAdmin = computed(() => {
  const role = userRole.value
  return role === 'Менеджер' || role === 'Администратор'
})

const isAuthenticatedUser = computed(() => {
  const user = userData.value
  return user && user.userRole === 'Авторизированный клиент'
})

const goToBasket = () => {
  router.push('/basket')
}

const goToOrders = () => {
  router.push('/orders')
}

const debounce = (fn, delay) => {
  let timeoutId
  return (...args) => {
    clearTimeout(timeoutId)
    timeoutId = setTimeout(() => fn(...args), delay)
  }
}

const debouncedFetchProducts = debounce(() => {
  if (isManagerOrAdmin.value) {
    fetchProducts()
  }
}, 500)

watch([searchQuery, selectedSupplier, selectedManufacturer, selectedCategory, sortOption], () => {
  if (isManagerOrAdmin.value) {
    debouncedFetchProducts()
  }
})

const fetchProducts = async () => {
  try {
    const params = {
      sortBy: sortOption.value
    }

    if (searchQuery.value.trim()) {
      params.search = searchQuery.value
    }
    if (selectedSupplier.value !== 'all') {
      params.supplierId = selectedSupplier.value
    }
    if (selectedManufacturer.value !== 'all') {
      params.manufacturerId = selectedManufacturer.value
    }
    if (selectedCategory.value !== 'all') {
      params.categoryId = selectedCategory.value
    }

    const response = await axios.get('http://localhost:5166/api/Tovar/GetAll', { params })
    
    if (Array.isArray(response.data)) {
      products.value = response.data
    } else {
      products.value = []
    }
  } catch (error) {
    console.error('Ошибка:', error)

    products.value = []

    // notification.value = {
    //   show: true,
    //   type: 'error',
    //   message: 'Ошибка при загрузке товаров'
    // }
    
    setTimeout(() => {
      notification.value.show = false
    }, 3000)
  }
}

const fetchProductsSimple = async () => {
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetAll')
    products.value = Array.isArray(response.data) ? response.data : []
  } catch (error) {
    console.error('Ошибка:', error)
    products.value = []
  }
}

const fetchSuppliers = async () => {
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetSuppliers')
    suppliers.value = response.data
  } catch (error) {
    console.error('Ошибка при загрузке поставщиков:', error)
  }
}

const fetchManufacturers = async () => {
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetManufacturers')
    manufacturers.value = response.data
  } catch (error) {
    console.error('Ошибка при загрузке производителей:', error)
  }
}

const fetchCategories = async () => {
  try {
    const response = await axios.get('http://localhost:5166/api/Tovar/GetCategories')
    categories.value = response.data
  } catch (error) {
    console.error('Ошибка при загрузке категорий:', error)
  }
}

const resetFilters = () => {
  searchQuery.value = ''
  selectedSupplier.value = 'all'
  selectedManufacturer.value = 'all'
  selectedCategory.value = 'all'
  sortOption.value = 'default'
}

const openAddModal = () => {
  modalMode.value = 'add'
  editingProduct.value = null
  showModal.value = true
}

const handleProductClick = (product) => {
  if (isAdmin.value) {
    modalMode.value = 'edit'
    editingProduct.value = product
    showModal.value = true
  }
}

const handleProductSaved = () => {
  showModal.value = false
  if (isManagerOrAdmin.value) {
    fetchProducts()
  } else {
    fetchProductsSimple()
  }
}

const handleProductDeleted = () => {
  showModal.value = false
  if (isManagerOrAdmin.value) {
    fetchProducts()
  } else {
    fetchProductsSimple()
  }
}

onMounted(() => {
  if (isManagerOrAdmin.value) {
    fetchProducts()
    fetchSuppliers()
    fetchManufacturers()
    fetchCategories()
  } else {
    fetchProductsSimple()
  }
})
</script>

<template>
  <div class="catalog">
    <transition name="notification">
      <div v-if="notification.show" :class="['notification-toast', notification.type]">
        <div class="notification-icon">
          <span v-if="notification.type === 'success'">✓</span>
          <span v-if="notification.type === 'error'">✗</span>
        </div>
        <div class="notification-content">
          {{ notification.message }}
        </div>
      </div>
    </transition>

    <div class="catalog-header">
      <div class="tov">
        <h1>Каталог товаров</h1>
        <div class="count_tovar">Всего товаров: {{ products.length }}</div>
      </div>
      
      <div class="header-actions">
        <!-- Кнопка Заказы для менеджеров и администраторов -->
        <button 
          v-if="isManagerOrAdmin" 
          @click="goToOrders" 
          class="orders-btn"
        >
          <span class="orders-icon">📋</span>
          Заказы
        </button>

        <button 
          v-if="isAuthenticatedUser" 
          @click="goToBasket" 
          class="basket-btn"
        >
          <span class="basket-icon">🛒</span>
          Перейти в корзину
        </button>
        
        <button 
          v-if="isAdmin" 
          @click="openAddModal" 
          class="add-product-btn"
        >
          + Добавить товар
        </button>
      </div>
    </div>

    <div v-if="isManagerOrAdmin" class="filters-panel">
      <div class="filter-group">
        <label for="search">Поиск:</label>
        <div class="search-wrapper">
          <input 
            id="search"
            type="text" 
            v-model="searchQuery" 
            placeholder="Название, производитель, поставщик..."
          />
          <span v-if="searchQuery" class="clear-search" @click="searchQuery = ''">×</span>
        </div>
      </div>

      <div class="filter-group">
        <label for="supplier">Поставщик:</label>
        <select id="supplier" v-model="selectedSupplier">
          <option value="all">Все поставщики</option>
          <option v-for="supplier in suppliers" :key="supplier.supplierId" :value="supplier.supplierId">
            {{ supplier.supplierName }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label for="manufacturer">Производитель:</label>
        <select id="manufacturer" v-model="selectedManufacturer">
          <option value="all">Все производители</option>
          <option v-for="manufacturer in manufacturers" :key="manufacturer.manufacturerId" :value="manufacturer.manufacturerId">
            {{ manufacturer.manufacturerName }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label for="category">Категория:</label>
        <select id="category" v-model="selectedCategory">
          <option value="all">Все категории</option>
          <option v-for="category in categories" :key="category.tovarCategoryId" :value="category.tovarCategoryId">
            {{ category.tovarCategoryName }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label for="sort">Сортировка:</label>
        <select id="sort" v-model="sortOption">
          <option value="default">По умолчанию</option>
          <option value="name_asc">От А до Я</option>
          <option value="name_desc">От Я до А</option>
          <option value="price_asc">Цена по возрастанию</option>
          <option value="price_desc">Цена по убыванию</option>
        </select>
      </div>

      <div class="filter-buttons">
        <button @click="resetFilters" class="reset-btn">Сбросить все фильтры</button>
      </div>
    </div>

    <div class="products-grid">
      <div
        v-for="product in products"
        
        :key="product.tovarArticle"
        :class="{ 'clickable-card': isAdmin }"
        @click="() => handleProductClick(product)"
      >
        <cardItem
          :product="product"
        />
      </div>
      
      <div v-if="products.length === 0" class="no-results">
        <div class="no-results-content">
          <svg class="no-results-icon" xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <circle cx="12" cy="12" r="10"></circle>
            <line x1="12" y1="8" x2="12" y2="12"></line>
            <line x1="12" y1="16" x2="12.01" y2="16"></line>
          </svg>
          <h3>Ничего не найдено</h3>
          <p>Попробуйте изменить параметры поиска</p>
        </div>
      </div>
    </div>

    <AddEditTovarModal
      v-if="showModal"
      :mode="modalMode"
      :product="editingProduct"
      :suppliers="suppliers"
      :manufacturers="manufacturers"
      :categories="categories"
      @close="showModal = false"
      @saved="handleProductSaved"
      @deleted="handleProductDeleted"
    />
  </div>
</template>

<style scoped>
.catalog {
  max-width: 1400px;
  margin: 0 auto;
  padding: 40px 20px;
}

.catalog-header {
  margin-top: 5%;
  margin-bottom: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.catalog-header h1 {
  font-size: 32px;
  font-weight: bold;
  color: #81694b;
  text-align: left;
}

.header-actions {
  display: flex;
  gap: 15px;
  align-items: center;
}

/* Стили для кнопки Заказы */
.orders-btn {
  background-color: #4a6f8c;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 12px 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}

.orders-btn:hover {
  background-color: #3a5a72;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(74, 111, 140, 0.3);
}

.orders-icon {
  font-size: 18px;
}

.basket-btn {
  background-color: #81694b;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 12px 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
  white-space: nowrap;
}

.basket-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px #715c42;
}

.basket-icon {
  font-size: 18px;
}

.add-product-btn {
  padding: 12px 25px;
  background-color: #81694b;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
  white-space: nowrap;
}

.add-product-btn:hover {
  background-color: #6d5a3e;
  transform: translateY(-2px);
}

.filters-panel {
  background: #f8f5f0;
  padding: 20px;
  border-radius: 10px;
  margin-bottom: 30px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 15px;
  align-items: end;
  width: 1190px;
}

.filter-group {
  display: flex;
  flex-direction: column;
  position: relative;
}

.filter-group label {
  margin-bottom: 8px;
  font-weight: 600;
  color: #81694b;
  font-size: 14px;
}

.search-wrapper {
  position: relative;
  width: 100%;
}

.filter-group input,
.filter-group select {
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  transition: border-color 0.3s;
  width: 100%;
}

.filter-group input:focus,
.filter-group select:focus {
  outline: none;
  border-color: #81694b;
}

.clear-search {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  cursor: pointer;
  font-size: 20px;
  color: #999;
  user-select: none;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  background: #f0f0f0;
}

.clear-search:hover {
  color: #333;
  background: #e0e0e0;
}

.filter-buttons {
  display: flex;
  gap: 10px;
  align-items: center;
  justify-content: flex-end;
}

.reset-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
  background-color: #f0f0f0;
  color: #333;
  white-space: nowrap;
}

.reset-btn:hover {
  background-color: #e0e0e0;
}

.clickable-card {
  cursor: pointer;
  transition: transform 0.2s;
}

.clickable-card:hover {
  transform: translateY(-5px);
}

.clickable-card:hover .product-card {
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  border-color: #81694b;
}

.no-results {
  grid-column: 1 / -1;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
}

.no-results-content {
  text-align: center;
  padding: 40px;
  background: #f9f9f9;
  border-radius: 12px;
  width: 1190px;
}

.no-results-icon {
  color: #81694b;
  opacity: 0.5;
  margin-bottom: 20px;
}

.no-results-content h3 {
  font-size: 24px;
  color: #333;
  margin-bottom: 10px;
}

.no-results-content p {
  color: #666;
  margin-bottom: 20px;
  line-height: 1.5;
}

.loading {
  text-align: center;
  padding: 40px;
  font-size: 18px;
  color: #666;
}

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

</style>