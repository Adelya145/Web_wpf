<script setup>
import { RouterLink, RouterView, useRouter } from 'vue-router'
import { ref, onMounted, computed, watch } from 'vue';
import { useCookies } from 'vue3-cookies';

const { cookies } = useCookies();
const userData = ref(null);
const router = useRouter();

const updateUserData = () => {
  userData.value = cookies.get('user');
  console.log('Обновлены данные пользователя:', userData.value);
};

watch(
  () => cookies.get('user'),
  (newValue) => {
    console.log('Cookie user изменился:', newValue);
    userData.value = newValue;
  }
);

const isGuest = computed(() => {
  return cookies.get('guest') === 'true';
});

const logout = () => {
  cookies.remove('user');
  cookies.remove('guest');
  userData.value = null;
  router.push('/');
};

onMounted(() => {
  updateUserData();
  
  router.afterEach(() => {
    setTimeout(() => {
      updateUserData();
    }, 50);
  });
});

const userInfo = computed(() => {
  if (!userData.value) return null;
  
  return {
    name: `${userData.value.userSurname} ${userData.value.userName} ${userData.value.userLastname}`,
    role: userData.value.userRole || 'Клиент'
  };
});
</script>

<template>
  <header class="header">
    <div class="top">
      <div class="logo">
        <img src="/resources/perfume-logo.avif" >
      </div>
      <div class="title">
        <h1 class="title_parfum">ООО Парфюм</h1>
      </div>
      
      <div class="user-info" v-if="userData">
        <div class="user-details">
          <span class="user-role">Вы вошли как {{ userInfo.role }}:</span>
          <span class="user-name">{{ userInfo.name }}</span>
        </div>
        <button @click="logout" class="logout-btn">Выйти</button>
      </div>
      
      <div class="menu" v-else>
        <RouterLink to="/" class="auth-link">Авторизация</RouterLink>
      </div>
    </div>
  </header>
  <RouterView class="content"/>
</template>

<style scoped>
.header {
  background: linear-gradient(135deg, #967bb6 0%, #b8a9d9 100%);
  padding: 15px 40px;
  box-shadow: 0 4px 12px rgba(150, 123, 182, 0.2);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
}

.top {
  display: grid;
  align-items: center;
  grid-template-columns: auto 900px auto auto;
  
  margin: 0 auto;
}

.logo {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  overflow: hidden;
  background-color: rgba(255, 255, 255, 0.1);

  border: 2px solid rgba(255, 255, 255, 0.3);
}

.logo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  border-radius: 50%;
}

.title {
  text-align: center;
}

.title_parfum {
  font-size: 28px;
  font-weight: 600;
  color: white;
  margin: 0;
}

.content {
  margin-top: 10%;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 20px;
}

.user-details {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  color: white;
  font-size: 14px;
  background-color: rgba(255, 255, 255, 0.1);
  padding: 8px 15px;
  border-radius: 6px;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.user-role {
  font-weight: 500;
  margin-bottom: 2px;
}

.user-name {
  font-weight: 600;
  font-size: 15px;
}

.logout-btn {
  background-color: rgba(255, 255, 255, 0.15);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 6px;
  padding: 10px 20px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.logout-btn:hover {
  background-color: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
}

.menu {
  display: flex;
  gap: 15px;
  align-items: center;
}

.menu a {
  color: white;
  text-decoration: none;
  font-size: 16px;
  font-weight: 600;
  padding: 10px 20px;
  background-color: rgba(255, 255, 255, 0.15);
  border-radius: 6px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  transition: all 0.3s ease;
}

.menu a:hover {
  background-color: rgba(255, 255, 255, 0.25);
  transform: translateY(-2px);
  cursor: pointer;
}

.auth-link {
  background-color: rgba(255, 255, 255, 0.25) !important;
}
</style>