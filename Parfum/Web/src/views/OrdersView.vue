<script setup>
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'
import { useCookies } from 'vue3-cookies'
import { useRouter } from 'vue-router'

const { cookies } = useCookies()
const router = useRouter()

const orders = ref([])
const loading = ref(false)
const selectedOrder = ref(null)
const showOrderDetails = ref(false)
const statusFilter = ref('all')

const notification = ref({
  show: false,
  type: '',
  message: ''
})

const userRole = computed(() => {
  const user = cookies.get('user')
  return user?.userRole || null
})

const isAdmin = computed(() => {
  return userRole.value === 'Администратор'
})

const isManager = computed(() => {
  return userRole.value === 'Менеджер'
})

const canEditStatus = computed(() => {
  return isAdmin.value
})

const statuses = [
  'Новый',
  'Завершен'
]

const filteredOrders = computed(() => {
  let result = orders.value

  // Фильтр по статусу
  if (statusFilter.value !== 'all') {
    result = result.filter(order => order.orderStatus === statusFilter.value)
  }

  return result
})

const fetchOrders = async () => {
  loading.value = true
  try {
    // Получаем все заказы (для админа и менеджера)
    const response = await axios.get('http://localhost:5166/api/Tovar/GetAllOrders')
    orders.value = response.data
  } catch (error) {
    console.error('Ошибка при загрузке заказов:', error)
    showNotification('error', 'Ошибка при загрузке заказов')
  } finally {
    loading.value = false
  }
}

const updateOrderStatus = async (orderId, newStatus) => {
  try {
    await axios.put(`http://localhost:5166/api/Tovar/UpdateOrderStatus/${orderId}`, {
      orderStatus: newStatus
    })
    
    // Обновляем статус в локальном массиве
    const order = orders.value.find(o => o.orderId === orderId)
    if (order) {
      order.orderStatus = newStatus
    }
    
    showNotification('success', 'Статус заказа успешно обновлен')
  } catch (error) {
    console.error('Ошибка при обновлении статуса:', error)
    showNotification('error', 'Ошибка при обновлении статуса заказа')
  }
}

const viewOrderDetails = (order) => {
  selectedOrder.value = order
  showOrderDetails.value = true
}

const closeOrderDetails = () => {
  showOrderDetails.value = false
  selectedOrder.value = null
}

const formatDate = (dateString) => {
  if (!dateString) return '—'
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU')
}

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('ru-RU', {
    style: 'currency',
    currency: 'RUB',
    minimumFractionDigits: 0
  }).format(amount)
}

const getStatusColor = (status) => {
  const colors = {
    'Новый': 'status-new',
    'Завершен': 'status-completed'
  }
  return colors[status] || ''
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

const Back = () => {
  router.push('/home')
}

onMounted(() => {
  if (isAdmin.value || isManager.value) {
    fetchOrders()
  } else {
    router.push('/')
  }
})
</script>

<template>
  <div class="orders-view">
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

    <div class="orders-header">
      <div class="header-title">
        <h1>Управление заказами</h1>
        <div class="user-role-badge" :class="{ 'admin': isAdmin, 'manager': isManager }">
          {{ isAdmin ? 'Администратор' : 'Менеджер' }}
        </div>
      </div>
      
      <div class="header-stats" v-if="!loading">
        <div class="stat-item">
          <span class="stat-label">Всего заказов:</span>
          <span class="stat-value">{{ orders.length }}</span>
        </div>
        <div class="stat-item">
          <span class="stat-label">Активных:</span>
          <span class="stat-value">{{ orders.filter(o => !['Завершен', 'Отменен'].includes(o.orderStatus)).length }}</span>
        </div>
      </div>
    </div>

    <div class="filters-section">
      <div class="filter-group">
        <label for="statusFilter">Статус:</label>
        <select id="statusFilter" v-model="statusFilter">
          <option value="all">Все статусы</option>
          <option v-for="status in statuses" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>

      <button @click="Back" class="reset-btn">Назад</button>
    </div>

    <div class="orders-table-container" v-if="!loading">
      <table class="orders-table">
        <thead>
          <tr>
            <th>Код заказа</th>
            <th>Код получения</th>
            <th>Дата создания</th>
            <th>Заказчик</th>
            <th>Пункт выдачи</th>
            <th>Статус</th>
            <th>Сумма</th>
            <th>Действия</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in filteredOrders" :key="order.orderId">
            <td class="order-id">{{ order.orderId }}</td>
            <td class="order-code">{{ order.orderCode }}</td>
            <td>{{ formatDate(order.orderDate) }}</td>
            <td>{{ order.userFull }}</td>
            <td class="address-cell">{{ order.pickUpPointAddress }}</td>
            <td>
              <div class="status-container">
                <span :class="['order-status', getStatusColor(order.orderStatus)]">
                  {{ order.orderStatus }}
                </span>
                <select 
                  v-if="canEditStatus && order.orderStatus !== 'Завершен'"
                  v-model="order.orderStatus"
                  @change="updateOrderStatus(order.orderId, order.orderStatus)"
                  class="status-edit"
                  :disabled="order.orderStatus === 'Завершен'"
                >
                  <option v-for="status in statuses" :key="status" :value="status">
                    {{ status }}
                  </option>
                </select>
              </div>
            </td>
            <td class="order-total">{{ formatCurrency(order.totalSum) }}</td>
            <td>
              <button @click="viewOrderDetails(order)" class="view-btn">
                Просмотр
              </button>
            </td>
          </tr>
          <tr v-if="filteredOrders.length === 0">
            <td colspan="7" class="no-results">
              Заказы не найдены
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else class="loading">
      Загрузка заказов...
    </div>

    <!-- Модальное окно с деталями заказа -->
    <div v-if="showOrderDetails" class="modal-overlay" @click="closeOrderDetails">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Заказ №{{ selectedOrder.orderId }}</h2>
          <button @click="closeOrderDetails" class="close-btn">×</button>
        </div>
        <div class="order-compositions">
          <h3>Состав заказа</h3>
          <table class="compositions-table">
            <thead>
              <tr>
                <th>Артикул</th>
                <th>Наименование</th>
                <th>Количество</th>
                <th>Цена за ед.</th>
                <th>Скидка</th>
                <th>Сумма</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in selectedOrder.items" :key="item.orderCompositionId">
                <td class="article">{{ item.tovarArticle }}</td>
                <td>{{ item.product.tovarName }}</td>
                <td>{{ item.orderCompositionCount }} шт.</td>
                <td>{{ formatCurrency(item.product.tovarCost) }}</td>
                <td>
                  <span v-if="item.product.tovarCurrentSale > 0" class="sale-badge">
                    -{{ item.product.tovarCurrentSale }}%
                  </span>
                  <span v-else>—</span>
                </td>
                <td class="item-total">{{ formatCurrency(item.totalPrice) }}</td>
              </tr>
            </tbody>
            <tfoot>
              <tr>
                <td colspan="5" class="total-label">Итого:</td>
                <td class="total-value">{{ formatCurrency(selectedOrder.totalSum) }}</td>
              </tr>
            </tfoot>
          </table>
        </div>

        <div v-if="canEditStatus && selectedOrder.orderStatus !== 'Завершен'" class="modal-footer">
          <label for="modalStatus">Изменить статус:</label>
          <select 
            id="modalStatus"
            v-model="selectedOrder.orderStatus"
            @change="updateOrderStatus(selectedOrder.orderId, selectedOrder.orderStatus)"
          >
            <option v-for="status in statuses" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.orders-view {
  max-width: 1400px;
  margin: 0 auto;
  padding: 40px 20px;
}

.orders-header {
  margin-top: 5%;
  margin-bottom: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.header-title {
  display: flex;
  align-items: center;
  gap: 20px;
}

.header-title h1 {
  font-size: 32px;
  font-weight: bold;
  color: #81694b;
}

.user-role-badge {
  padding: 6px 15px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 600;
  color: white;
}

.user-role-badge.admin {
  background-color: #c44545;
}

.user-role-badge.manager {
  background-color: #4580c4;
}

.header-stats {
  display: flex;
  gap: 30px;
  background: #f8f5f0;
  padding: 15px 25px;
  border-radius: 10px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-label {
  font-size: 14px;
  color: #666;
  margin-bottom: 5px;
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  color: #81694b;
}

.filters-section {
  background: #f8f5f0;
  padding: 20px;
  border-radius: 10px;
  margin-bottom: 30px;
  display: flex;
  gap: 20px;
  align-items: flex-end;
  flex-wrap: wrap;
  justify-content: space-between;
}

.filter-group {
  display: flex;
  flex-direction: column;
  min-width: 200px;
}

.filter-group label {
  margin-bottom: 8px;
  font-weight: 600;
  color: #81694b;
  font-size: 14px;
}

.filter-group input,
.filter-group select {
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  transition: border-color 0.3s;
}

.filter-group input:focus,
.filter-group select:focus {
  outline: none;
  border-color: #81694b;
}

.reset-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  background-color: #e0e0e0;
  color: #333;
  white-space: nowrap;
  height: 40px;
}

.reset-btn:hover {
  background-color: #d0d0d0;
}

.orders-table-container {
  background: white;
  border-radius: 10px;
  overflow-x: auto;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.orders-table {
  border-collapse: collapse;
  width: 1200px;
}

.orders-table th {
  background: #81694b;
  color: white;
  padding: 15px;
  text-align: left;
  font-weight: 600;
}

.orders-table td {
  padding: 15px;
  border-bottom: 1px solid #eee;
}

.orders-table tr:hover {
  background: #f9f9f9;
}

.order-code {
  font-weight: 600;
  color: #81694b;
}

.address-cell {
  max-width: 250px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.status-container {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}

.order-status {
  padding: 5px 10px;
  border-radius: 15px;
  font-size: 13px;
  font-weight: 500;
  text-align: center;
  white-space: nowrap;
}

.status-new {
  background: #e3f2fd;
  color: #1976d2;
}

.status-processing {
  background: #fff3e0;
  color: #f57c00;
}

.status-collecting {
  background: #f3e5f5;
  color: #7b1fa2;
}

.status-delivery {
  background: #e8f5e8;
  color: #388e3c;
}

.status-ready {
  background: #e0f7fa;
  color: #0097a7;
}

.status-completed {
  background: #e8f5e9;
  color: #2e7d32;
}

.status-cancelled {
  background: #ffebee;
  color: #c62828;
}

.status-edit {
  padding: 5px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 12px;
  background: white;
  cursor: pointer;
}

.status-edit:disabled {
  background: #f5f5f5;
  cursor: not-allowed;
  opacity: 0.6;
}

.order-total {
  font-weight: 600;
  color: #333;
}

.view-btn {
  padding: 6px 15px;
  background: #81694b;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s;
  font-size: 13px;
}

.view-btn:hover {
  background: #6d5a3e;
}

.no-results {
  text-align: center;
  padding: 40px;
  color: #666;
  font-style: italic;
}

.loading {
  text-align: center;
  padding: 60px;
  font-size: 18px;
  color: #81694b;
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
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 10px;
  width: 100%;
  max-width: 900px;
  max-height: 90vh;
  overflow-y: auto;
  position: relative;
  animation: modalSlideIn 0.3s ease-out;
}

@keyframes modalSlideIn {
  from {
    transform: translateY(-30px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  padding: 20px;
  border-bottom: 2px solid #f0f0f0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 0;
  background: white;
  z-index: 10;
}

.modal-header h2 {
  color: #81694b;
  font-size: 24px;
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 30px;
  cursor: pointer;
  color: #666;
  padding: 0;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.3s;
}

.close-btn:hover {
  background: #f0f0f0;
  color: #333;
}


.order-compositions {
  padding: 0 20px 20px;
}

.order-compositions h3 {
  color: #81694b;
  margin-bottom: 15px;
  font-size: 18px;
}

.compositions-table {
  width: 100%;
  border-collapse: collapse;
  border: 1px solid #eee;
  border-radius: 8px;
  overflow: hidden;
}

.compositions-table th {
  background: #f0f0f0;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: #333;
}

.compositions-table td {
  padding: 12px;
  border-bottom: 1px solid #eee;
}

.compositions-table tbody tr:hover {
  background: #f9f9f9;
}

.article {
  font-family: monospace;
  font-weight: 600;
  color: #81694b;
}

.sale-badge {
  background: #ff6b6b;
  color: white;
  padding: 3px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
  white-space: nowrap;
}

.item-total {
  font-weight: 600;
  color: #333;
}

.compositions-table tfoot {
  background: #f8f5f0;
}

.compositions-table tfoot td {
  padding: 15px 12px;
  font-weight: 600;
}

.total-label {
  text-align: right;
  color: #666;
}

.total-value {
  font-size: 18px;
  color: #81694b;
  font-weight: bold;
}

.modal-footer {
  padding: 20px;
  border-top: 2px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 15px;
  background: white;
  position: sticky;
  bottom: 0;
}

.modal-footer label {
  font-weight: 600;
  color: #81694b;
}

.modal-footer select {
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  flex: 1;
  max-width: 300px;
}

.modal-footer select:focus {
  outline: none;
  border-color: #81694b;
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