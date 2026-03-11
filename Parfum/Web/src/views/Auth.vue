<script setup>
import { ref } from 'vue';
import { useCookies } from 'vue3-cookies';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const { cookies } = useCookies();

const email = ref('');
const password = ref('');

async function auth() { 
  try {
    if(password.value == '' || email.value == ''){
      alert("Поля не должны быть пустыми!")
      return
    }
    
    const response = await axios.post('http://localhost:5166/api/User/Authorization', 
      null,
      {
        params: {
          email: email.value,
          password: password.value
        }
      }
    );
    
    if (response.data) {
      alert("Вы успешно вошли в профиль!");
      console.log('Пользователь:', response.data);
      
      cookies.set('user', response.data);
      
      cookies.remove('guest');
      
      router.push('/home');
    } else {
      alert('Ошибка: Неверные данные пользователя');
    }
  }
  catch(error) {
    console.error('Ошибка авторизации:', error);
    if (error.response) {
      const errorMessage = error.response.data;
      alert('Ошибка: ' + errorMessage);
    } else if (error.request) {
      alert('Ошибка: Сервер не отвечает');
    } else {
      alert('Ошибка: ' + error.message);
    }
  }
}

const loginAsGuest = () => {
  cookies.set('guest', 'true', '1h');
  cookies.remove('user');
  router.push('/home');
}
</script>

<template>
  <main class="main">
    <div class="login-container">
      <div class="auth-header">
        <h1>Вход в аккаунт</h1>
      </div>
      
      <form @submit.prevent="auth" class="auth-form">
        <div class="input-form">
          <label class="input-label">Логин или Email</label>
          <input 
            type="text" 
            v-model="email" 
            placeholder="Введите логин или email" 
            required 
            class="form-input"
          >
        </div>
        
        <div class="input-form">
          <label class="input-label">Пароль</label>
          <input 
            type="password" 
            v-model="password" 
            placeholder="Введите пароль"
            required 
            class="form-input"
          >
        </div>
        
        <button type="submit" class="auth-btn">
          Войти
        </button>
        
        <div class="guest-section">
          <p class="guest-text">Или</p>
          <button @click="loginAsGuest" type="button" class="guest-btn">
            Войти как гость
          </button>
        </div>
      </form>
    </div>
  </main>
</template>

<style scoped>
.main {
  display: flex;
  justify-content: center;
  padding: 30px 20px 60px;
}

.login-container {
  display: flex;
  width: 90vh;
  flex-direction: column;
  align-items: center;
  padding: 30px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(150, 123, 182, 0.15);
  border: 1px solid #e8e4f2;
}

.auth-header h1 {
  font-size: 30px;
  font-weight: 600;
  margin-bottom: 25px;
  color: #555;
  text-align: center;
}

.auth-form {
  width: 100%;
  margin: 20px 0;
}

.input-form {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
}

.input-label {
  font-size: 18px;
  font-weight: 500;
  margin-bottom: 8px;
  color: #666;
}

.form-input {
  background-color: #f9f7fc;
  border-radius: 6px;
  border: 1px solid #d8d0e6;
  font-size: 18px;
  padding: 12px 15px;
  width: 100%;
  box-sizing: border-box;
  color: #333;
}

.form-input:focus {
  outline: none;
  border-color: #967bb6;
  background-color: #fff;
}

.form-input::placeholder {
  color: #999;
  font-size: 14px;
}

.auth-btn {
  background: linear-gradient(135deg, #967bb6 0%, #b8a9d9 100%);
  color: white;
  border-radius: 6px;
  border: none;
  font-size: 16px;
  font-weight: 500;
  padding: 14px 30px;
  width: 100%;
  cursor: pointer;
  transition: background 0.3s;
}

.auth-btn:hover {
  background: #8569a3;
}

.guest-section {
  margin-top: 30px;
  text-align: center;
  width: 100%;
}

.guest-text {
  color: #666;
  margin-bottom: 15px;
  font-size: 14px;
}

.guest-btn {
  background: transparent;
  color: #967bb6;
  border: 2px solid #967bb6;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 500;
  padding: 12px 30px;
  width: 100%;
  cursor: pointer;
  transition: all 0.3s;
}

.guest-btn:hover {
  background-color: #967bb6;
  color: white;
}
</style>