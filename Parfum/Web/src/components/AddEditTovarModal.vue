<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const props = defineProps({
  mode: { type: String, default: 'add' },
  product: { type: Object, default: null },
  suppliers: { type: Array, default: () => [] },
  manufacturers: { type: Array, default: () => [] },
  categories: { type: Array, default: () => [] }
})

const emit = defineEmits(['close', 'saved', 'deleted'])

const form = ref({
  tovarArticle: '',
  tovarName: '',
  tovarDesc: '',
  tovarCost: 0,
  tovarCount: 0,
  tovarCurrentSale: 0,
  tovarUnit: 'шт.',
  supplierId: '',
  manufactrurerId: '',
  tovarCategoryId: '',
  tovarPhoto: ''
})

const loading = ref(false)
const errorMessage = ref('')
const imageFile = ref(null)
const imagePreview = ref('')

const modalTitle = computed(() => 
  props.mode === 'add' ? 'Добавить товар' : 'Редактировать товар'
)

const showArticleField = computed(() => props.mode === 'edit')

onMounted(() => {
  if (props.mode === 'edit' && props.product) {
    form.value = { ...props.product }
    form.value.tovarUnit = 'шт.'
    if (form.value.tovarPhoto) {
      const fileName = form.value.tovarPhoto.split('/').pop()
      imagePreview.value = `/resources/${fileName}`
      form.value.tovarPhoto = fileName
    }
  }
})

const onImageSelect = (event) => {
  const file = event.target.files[0]
  if (!file) return

  if (file.size > 5 * 1024 * 1024) {
    errorMessage.value = 'Файл слишком большой (максимум 5 MB)'
    return
  }

  const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp']
  if (!allowedTypes.includes(file.type)) {
    errorMessage.value = 'Недопустимый формат файла. Разрешены: JPG, PNG, GIF, WebP'
    return
  }

  imageFile.value = file

  const reader = new FileReader()
  reader.onload = (e) => {
    imagePreview.value = e.target.result
  }
  reader.readAsDataURL(file)
}

const removeImage = () => {
  imageFile.value = null
  imagePreview.value = null
  form.value.tovarPhoto = ''
}

const saveProduct = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    form.value.tovarUnit = 'шт.'

    if (imageFile.value) {
      const imageForm = new FormData()
      imageForm.append('image', imageFile.value)

      const imageResponse = await axios.post(
        'http://localhost:5166/api/Tovar/UploadImage',
        imageForm,
        {
          headers: { 'Content-Type': 'multipart/form-data' }
        }
      )

      if (imageResponse.data.success) {
        form.value.tovarPhoto = imageResponse.data.fileName
      }
    }

    const url = props.mode === 'add' 
      ? 'http://localhost:5166/api/Tovar/AddTovar'
      : 'http://localhost:5166/api/Tovar/RedactTovar'

    const response = await axios({
      method: props.mode === 'add' ? 'POST' : 'PUT',
      url: url,
      data: form.value
    })

    if (props.mode === 'add') {
      alert(`✅ ${response.data.message}\n\nАртикул товара: ${response.data.article}\nНазвание: ${response.data.name}`)
    } else {
      alert(`✅ ${response.data.message}`)
    }
    
    emit('saved')

  } catch (error) {
    errorMessage.value = error.response?.data?.message || error.response?.data || 'Ошибка сохранения'
    console.error('Ошибка:', error)
  } finally {
    loading.value = false
  }
}

const deleteProduct = async () => {
  if (!confirm('Удалить этот товар?')) return

  loading.value = true
  try {
    await axios.delete('http://localhost:5166/api/Tovar/DeleteTovar', {
      params: { article: form.value.tovarArticle }
    })
    
    alert('✅ Товар успешно удален')
    emit('deleted')
  } catch (error) {
    errorMessage.value = error.response?.data || 'Ошибка удаления'
  } finally {
    loading.value = false
  }
}

const closeModal = () => {
  emit('close')
}
</script>

<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <div class="modal-header">
        <h2>{{ modalTitle }}</h2>
        <button class="close-btn" @click="closeModal">×</button>
      </div>

      <div class="modal-body">
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <form @submit.prevent="saveProduct" class="product-form">
          <div v-if="showArticleField" class="form-group">
            <label>Артикул</label>
            <input v-model="form.tovarArticle" readonly class="readonly-field">
          </div>

          <div class="form-group">
            <label>Название *</label>
            <input v-model="form.tovarName" required maxlength="35">
          </div>

          <div class="form-group">
            <label>Описание *</label>
            <textarea v-model="form.tovarDesc" required rows="3"></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Цена *</label>
              <input v-model.number="form.tovarCost" type="number" required min="0" step="0.01">
            </div>
            <div class="form-group">
              <label>Количество *</label>
              <input v-model.number="form.tovarCount" type="number" required min="0">
            </div>
            <div class="form-group">
              <label>Скидка (%)</label>
              <input v-model.number="form.tovarCurrentSale" type="number" min="0" max="100">
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label>Поставщик *</label>
              <select v-model="form.supplierId" required>
                <option value="">Выберите поставщика</option>
                <option v-for="s in suppliers" :key="s.supplierId" :value="s.supplierId">
                  {{ s.supplierName }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label>Производитель *</label>
              <select v-model="form.manufactrurerId" required>
                <option value="">Выберите производителя</option>
                <option v-for="m in manufacturers" :key="m.manufacturerId" :value="m.manufacturerId">
                  {{ m.manufacturerName }}
                </option>
              </select>
            </div>
            <div class="form-group">
              <label>Категория *</label>
              <select v-model="form.tovarCategoryId" required>
                <option value="">Выберите категорию</option>
                <option v-for="c in categories" :key="c.tovarCategoryId" :value="c.tovarCategoryId">
                  {{ c.tovarCategoryName }}
                </option>
              </select>
            </div>
          </div>

          <div class="form-group">
            <label>Единица измерения *</label>
            <input 
              v-model="form.tovarUnit" 
              readonly 
              class="readonly-field unit-field"
              value="шт."
            >
          </div>

          <div class="form-group">
            <label>Изображение</label>
            <div class="image-upload">
              <input type="file" @change="onImageSelect" accept="image/*">
              <p class="hint">Максимум 5 MB. Допустимы: JPG, PNG, GIF, WebP</p>
              <div v-if="imagePreview" class="preview">
                <img :src="imagePreview" alt="Превью">
                <button type="button" @click="removeImage" class="remove-image-btn">×</button>
              </div>
              <div v-else-if="form.tovarPhoto && mode === 'edit'" class="preview">
                <img :src="'/resources/' + form.tovarPhoto" alt="Текущее изображение">
                <button type="button" @click="removeImage" class="remove-image-btn">×</button>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" @click="closeModal" :disabled="loading">
              Отмена
            </button>
            
            <button v-if="mode === 'edit'" type="button" @click="deleteProduct" 
                    class="delete-btn" :disabled="loading">
              Удалить
            </button>
            
            <button type="submit" :disabled="loading" class="save-btn">
              {{ loading ? 'Сохранение...' : (mode === 'add' ? 'Добавить' : 'Сохранить') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
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
  z-index: 9999;
  padding: 20px;
}

.modal-content {
  background: #f8f5ff;
  border-radius: 12px;
  width: 100%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 30px rgba(150, 123, 182, 0.3);
  border: 2px solid #967bb6;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  background: linear-gradient(135deg, #967bb6 0%, #b8a9d9 100%);
  color: white;
  border-radius: 10px 10px 0 0;
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
}

.close-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: none;
  width: 36px;
  height: 36px;
  font-size: 1.5rem;
  cursor: pointer;
  transition: 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 15%;
}

.close-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(90deg);
}

.modal-body {
  padding: 25px;
}

.product-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-row {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.form-row .form-group {
  flex: 1;
  min-width: 150px;
}

label {
  font-weight: 600;
  color: #5d4a7a;
  font-size: 0.95rem;
}

input, select, textarea {
  padding: 12px 15px;
  border: 1px solid #d1c4e9;
  border-radius: 8px;
  font-size: 0.95rem;
  background: white;
  transition: 0.2s;
  color: #333;
}

input:focus, select:focus, textarea:focus {
  outline: none;
  border-color: #967bb6;
  box-shadow: 0 0 0 3px rgba(150, 123, 182, 0.2);
}

.readonly-field {
  background-color: #f0f0f0;
  cursor: not-allowed;
  border: 1px solid #d1c4e9;
  color: #5d4a7a;
  font-weight: 500;
}

.unit-field {
  background-color: #e8e0f5;
  font-weight: 600;
  color: #5d4a7a;
}

textarea {
  resize: vertical;
  min-height: 80px;
}

.hint {
  font-size: 0.85rem;
  color: #7e7e7e;
  margin: 5px 0 0 0;
  font-style: italic;
}

.image-upload {
  margin-top: 5px;
}

.image-upload input[type="file"] {
  padding: 8px;
  background: white;
  border-radius: 8px;
  border: 2px dashed #d1c4e9;
  width: 100%;
  cursor: pointer;
}

.image-upload input[type="file"]:hover {
  border-color: #967bb6;
}

.preview {
  position: relative;
  width: 200px;
  height: 200px;
  margin-top: 15px;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 3px 10px rgba(0, 0, 0, 0.15);
  border: 2px solid #e6d9f2;
}

.preview img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.remove-image-btn {
  position: absolute;
  top: 5px;
  right: 5px;
  width: 30px;
  height: 30px;
  background: rgba(220, 53, 69, 0.8);
  color: white;
  border: none;
  border-radius: 50%;
  font-size: 1.2rem;
  cursor: pointer;
  transition: 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: auto;
  padding: 0;
}

.remove-image-btn:hover {
  background: #dc3545;
  transform: scale(1.1);
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  margin-top: 25px;
  padding-top: 20px;
  border-top: 1px solid #e6d9f2;
  gap: 15px;
}

button {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.95rem;
  font-weight: 600;
  transition: all 0.3s;
  min-width: 100px;
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none !important;
}

button:not(:disabled):hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.save-btn {
  background: rgb(150, 77, 199);
  color: white;
  padding: 12px 28px;
}

.save-btn:hover:not(:disabled) {
  background: rgb(114, 58, 151);
}

.delete-btn {
  background: linear-gradient(135deg, #dc3545 0%, #e35d6a 100%);
  color: white;
}

.delete-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #c82333 0%, #d54c59 100%);
}

.form-actions button:first-child {
  background: #6c757d;
  color: white;
}

.form-actions button:first-child:hover:not(:disabled) {
  background: #5a6268;
}

.error-message {
  background: #f8d7da;
  color: #721c24;
  padding: 12px 15px;
  border-radius: 8px;
  margin-bottom: 20px;
  border: 1px solid #f5c6cb;
  font-size: 0.9rem;
}

input:invalid, select:invalid, textarea:invalid {
  border-color: #dc3545;
  background-color: #fff8f8;
}
</style>