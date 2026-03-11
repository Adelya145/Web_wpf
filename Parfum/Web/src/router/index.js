import { createRouter, createWebHistory } from 'vue-router'
import home from '@/views/MainWindow.vue'
import LoginPage from '@/views/Auth.vue'
import  basket from '@/views/BascetView.vue'
import orders from '@/views/OrdersView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'Login',
      component: LoginPage,
    },
    {
      path: '/home',
      name: 'home',
      component: home
    },
    {
      path: '/basket',
      name: 'basket',
      component: basket
    },
    {
      path: '/orders',
      name: 'orders',
      component: orders
    }

  ],
})

export default router
