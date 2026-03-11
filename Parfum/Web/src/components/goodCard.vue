<script setup>
import { computed } from 'vue'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'

const { cookies } = useCookies()

const props = defineProps({
  product: {
    type: Object,
    required: true,
    default: () => ({})
  }
})

const emit = defineEmits(['added-to-cart', 'error'])

const userData = computed(() => {
  return cookies.get('user')
})

const isAuthenticatedUser = computed(() => {
  const user = userData.value
  return user && user.userRole === 'Авторизированный клиент'
})

const canAddToCart = computed(() => {
  return isAuthenticatedUser.value && props.product.tovarCount > 0
})

const imagePath = computed(() => {
  const image = props.product.tovarPhoto || 'picture.png'
  return `/resources/${image}`
})

const discountedPrice = computed(() => {
  const cost = props.product.tovarCost
  const discount = props.product.tovarCurrentSale
  
  if (discount > 0) {
    return Math.round(cost * (1 - discount / 100))
  }
  return cost
})

const hasDiscount = computed(() => {
  return (props.product.tovarCurrentSale || 0) > 0
})

const isBigDiscount = computed(() => {
  return (props.product.tovarCurrentSale || 0) >= 15
})

const isOutOfStock = computed(() => {
  return (props.product.tovarCount || 0) === 0
})

const addToCart = async () => {
  if (!isAuthenticatedUser.value) {
    emit('error', 'Только авторизованные пользователи могут добавлять товары в корзину')
    return
  }

  if (props.product.tovarCount <= 0) {
    emit('error', 'Товар отсутствует на складе')
    return
  }

  try {
    const response = await axios.post('http://localhost:5166/api/Tovar/AddToBasket', {
      userId: userData.value.userId,
      tovarArticle: props.product.tovarArticle,
      quantity: 1
    })

    if (response.data.success) {
      emit('added-to-cart', {
        message: response.data.message,
        product: props.product
      })
    }
  } catch (error) {
    const errorMessage = error.response?.data || 'Ошибка при добавлении в корзину'
    emit('error', errorMessage)
    console.error('Ошибка добавления в корзину:', error)
  }
}
</script>

<template>
  <div class="product-card" v-if="product" :class="{ 'big-discount': isBigDiscount }">
    <div class="column image-column">
      <div class="product-image">
        <img 
          :src="imagePath" 
          :alt="product.tovarName"
          class="perfume-image"
          @error="(e) => { e.target.src = '/resources/picture.png' }"
        />
      </div>
    </div>
    
    <div class="column info-column">
      <div class="category-row">
        <span class="product-category">{{ product.tovarCategoryName }}</span>
        <span class="separator">|</span>
        <span class="product-name">{{ product.tovarName }}</span>
      </div>
      
      <div class="description-row">
        <span class="label">Описание товара:</span>
        <span class="value">{{ product.tovarDesc }}</span>
      </div>
      
      <div class="manufacturer-row">
        <span class="label">Производитель:</span>
        <span class="value">{{ product.manufacturerName}}</span>
      </div>
      
      <div class="supplier-row">
        <span class="label">Поставщик:</span>
        <span class="value">{{ product.supplierName }}</span>
      </div>
      
      <div class="price-row">
        <span class="label">Цена:</span>
        <div class="price-value">
          <template v-if="hasDiscount">
            <span class="original-price">{{ (product.tovarCost).toLocaleString() }}₽</span>
            <span class="arrow">→</span>
            <span class="discounted-price">{{ discountedPrice.toLocaleString() }}₽</span>
          </template>
          <template v-else>
            <span class="current-price">{{ (product.tovarCost).toLocaleString() }}₽</span>
          </template>
        </div>
      </div>
      
      <div class="unit-info">
        <span class="label">Единица измерения:</span>
        <span class="value">{{ product.tovarUnit || 'шт.' }}</span>
      </div>
      
      <div class="stock-info" :class="{ 'out-of-stock': isOutOfStock }">
        <span class="label">Количество на складе:</span>
        <span class="value">{{ product.tovarCount || 0 }}</span>
      </div>

      <!-- Кнопка добавления в корзину -->
      <div class="cart-action" v-if="canAddToCart">
        <button @click="addToCart" class="add-to-cart-btn">
          <span class="cart-icon">🛒</span>
          Добавить в корзину
        </button>
      </div>
      <div v-else-if="isAuthenticatedUser && isOutOfStock" class="out-of-stock-message">
        Нет в наличии
      </div>
    </div>
    
    <div class="column discount-column" v-if="hasDiscount">
      <div class="discount-circle">
        <div class="discount-percent">-{{ product.tovarCurrentSale }}%</div>
        <div class="discount-label">СКИДКА</div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-card {
  display: grid;
  grid-template-columns: 150px 1fr 120px;
  gap: 15px;
  border: 1px solid #e7e7e7;
  border-radius: 8px;
  padding: 15px;
  background: white;
  transition: all 0.3s ease;
  min-height: 200px;
  align-self: center;
  height: 100%;
  margin: 15px 0;
}

.product-card:hover {
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);
  transform: translateY(-3px);
  border-color: #d4a574;
}

.product-card.big-discount {
  background: #f8f5ff;
  border: 2px solid #967bb6;
  border-left: 5px solid #967bb6;
}

.stock-info.out-of-stock {
  background-color: #967bb6;
  color: white;
  padding: 3px 8px;
  border-radius: 4px;
  margin-top: 5px;
}

.stock-info.out-of-stock .label,
.stock-info.out-of-stock .value {
  color: white;
  font-weight: 700;
}

.column.image-column {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.product-image {
  width: 100%;
  height: 140px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.perfume-image {
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-radius: 6px;
  background: linear-gradient(135deg, #f9f5f0 0%, #f0e6d6 100%);
  padding: 8px;
}

.column.info-column {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding-right: 10px;
}

.category-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 5px;
  flex-wrap: wrap;
}

.product-category {
  font-size: 14px;
  font-weight: 600;
  color: #8b7355;
  text-transform: uppercase;
}

.separator {
  color: #ccc;
  font-size: 16px;
}

.product-name {
  font-size: 14px;
  color: #333;
  font-weight: 500;
}

.description-row {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  font-size: 13px;
  line-height: 1.3;
}

.description-row,
.manufacturer-row,
.supplier-row,
.price-row {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  font-size: 13px;
  line-height: 1.3;
}

.label {
  color: #666;
  font-weight: 500;
  white-space: nowrap;
  min-width: 120px;
  flex-shrink: 0;
}

.value {
  color: #333;
  font-weight: 600;
  flex: 1;
  word-break: break-word;
}

.description-row .value {
  white-space: normal;
  line-height: 1.4;
}

.price-row {
  margin-top: 2px;
}

.price-row .label {
  padding-top: 2px;
}

.price-value {
  display: flex;
  align-items: center;
  gap: 6px;
  flex-wrap: wrap;
}

.original-price {
  font-size: 15px;
  color: #999;
  text-decoration: line-through;
  font-weight: 500;
}

.arrow {
  color: #8b7355;
  font-size: 14px;
  font-weight: bold;
}

.discounted-price,
.current-price {
  font-size: 18px;
  font-weight: 700;
  color: #b8860b;
}

.unit-info,
.stock-info {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 12px;
  margin-top: 2px;
}

.unit-info .label,
.stock-info .label {
  min-width: auto;
  color: #777;
}

.unit-info .value,
.stock-info .value {
  color: #555;
  font-weight: 600;
}

.column.discount-column {
  display: flex;
  align-items: center;
  justify-content: center;
  padding-left: 10px;
  border-left: 1px solid #eee;
}

.discount-circle {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 53px;
  height: 53px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ff6b6b 0%, #ff8e8e 100%);
  color: white;
  box-shadow: 0 4px 10px rgba(255, 107, 107, 0.3);
}

.discount-percent {
  font-size: 16px;
  font-weight: 400;
  line-height: 1;
}

.discount-label {
  font-size: 10px;
  font-weight: 600;
  letter-spacing: 0.5px;
  margin-top: 2px;
  text-transform: uppercase;
}

/* Стили для кнопки корзины */
.cart-action {
  margin-top: 10px;
}

.add-to-cart-btn {
  background: linear-gradient(135deg, #967bb6 0%, #b8a9d9 100%);
  color: white;
  border: none;
  border-radius: 6px;
  padding: 8px 15px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
}

.add-to-cart-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(150, 123, 182, 0.3);
}

.add-to-cart-btn:active {
  transform: translateY(0);
}

.cart-icon {
  font-size: 16px;
}

.out-of-stock-message {
  margin-top: 10px;
  color: #dc3545;
  font-size: 14px;
  font-weight: 600;
  text-align: center;
  padding: 5px;
  background-color: #fff5f5;
  border-radius: 4px;
  border: 1px solid #ffcdd2;
}
</style>