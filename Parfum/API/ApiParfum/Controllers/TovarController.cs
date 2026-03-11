using ApiParfum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ApiParfum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TovarController : ControllerBase
    {
        private const int ARTICLE_LENGTH = 6;
        private const string ALLOWED_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random();

        // Константы для изображения
        private const int IMAGE_WIDTH = 300;
        private const int IMAGE_HEIGHT = 200;
        private const int IMAGE_MAX_SIZE = 5 * 1024 * 1024; // 5 MB

        [HttpGet("GetAll")]
        public ActionResult<Tovar> GetAll(
            [FromQuery] string? sortBy = "default",
            [FromQuery] string? search = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] int? manufacturerId = null,
            [FromQuery] int? categoryId = null)
        {
            using (var context = new DbParfumeContext())
            {
                var goods = context.Tovars
                    .Include(b => b.Manufactrurer)
                    .Include(c => c.Supplier)
                    .Include(c => c.TovarCategory)
                    .AsQueryable();

                if (supplierId.HasValue)
                    goods = goods.Where(t => t.SupplierId == supplierId.Value);

                if (manufacturerId.HasValue)
                    goods = goods.Where(t => t.ManufactrurerId == manufacturerId.Value);

                if (categoryId.HasValue)
                    goods = goods.Where(t => t.TovarCategoryId == categoryId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim().ToLower();
                    goods = goods.Where(t =>
                        t.TovarName.ToLower().Contains(search) ||
                        t.TovarDesc.ToLower().Contains(search) ||
                        (t.Manufactrurer != null && t.Manufactrurer.ManufacturerName.ToLower().Contains(search)) ||
                        (t.Supplier != null && t.Supplier.SupplierName.ToLower().Contains(search)) ||
                        (t.TovarCategory != null && t.TovarCategory.TovarCategoryName.ToLower().Contains(search))
                    );
                }

                goods = sortBy.ToLower() switch
                {
                    "name_asc" => goods.OrderBy(t => t.TovarName),
                    "name_desc" => goods.OrderByDescending(t => t.TovarName),
                    "price_asc" => goods.OrderBy(t => t.TovarCost),
                    "price_desc" => goods.OrderByDescending(t => t.TovarCost),
                    _ => goods.OrderByDescending(t => t.TovarArticle)
                };

                var products = goods.ToList();

                if (products == null || products.Count == 0)
                    return NotFound("Товары не найдены.");

                return Ok(products);
            }
        }

        [HttpGet("GetSuppliers")]
        public ActionResult<Supplier> GetSuppliers()
        {
            using (var context = new DbParfumeContext())
                return Ok(context.Suppliers.ToList());
        }

        [HttpGet("GetManufacturers")]
        public ActionResult<Manufacturer> GetManufacturers()
        {
            using (var context = new DbParfumeContext())
                return Ok(context.Manufacturers.ToList());
        }

        [HttpGet("GetCategories")]
        public ActionResult<TovarCategory> GetCategories()
        {
            using (var context = new DbParfumeContext())
                return Ok(context.TovarCategories.ToList());
        }

        [HttpGet("GetTovarId")]
        public ActionResult<Tovar> GetTovarId(string article)
        {
            using (var context = new DbParfumeContext())
            {
                var product = context.Tovars
                    .Include(b => b.Manufactrurer)
                    .Include(c => c.Supplier)
                    .Include(c => c.TovarCategory)
                    .FirstOrDefault(x => x.TovarArticle == article);

                return product == null
                    ? NotFound("Товар не найден.")
                    : Ok(product);
            }
        }

        private string GenerateUniqueArticle(DbParfumeContext context)
        {
            int maxAttempts = 100;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                var article = GenerateRandomArticle();

                bool exists = context.Tovars.Any(t => t.TovarArticle == article);

                if (!exists)
                    return article;

                attempt++;
            }

            return GenerateArticleWithTimestamp();
        }

        private string GenerateRandomArticle()
        {
            var sb = new StringBuilder(ARTICLE_LENGTH);
            for (int i = 0; i < ARTICLE_LENGTH; i++)
            {
                int index = random.Next(ALLOWED_CHARS.Length);
                sb.Append(ALLOWED_CHARS[index]);
            }
            return sb.ToString();
        }

        private string GenerateArticleWithTimestamp()
        {
            var timestamp = DateTime.Now.Ticks.ToString().Substring(10, 4);
            var sb = new StringBuilder();
            for (int i = 0; i < ARTICLE_LENGTH - timestamp.Length; i++)
            {
                int index = random.Next(ALLOWED_CHARS.Length);
                sb.Append(ALLOWED_CHARS[index]);
            }

            sb.Append(timestamp);

            return sb.ToString();
        }

        [HttpPost("AddTovar")]
        public ActionResult Post([FromBody] Tovar tovar)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (tovar == null)
                        return BadRequest("Данные товара не предоставлены.");

                    var generatedArticle = GenerateUniqueArticle(context);

                    if (string.IsNullOrWhiteSpace(tovar.TovarName))
                        return BadRequest("Название товара является обязательным полем.");

                    if (tovar.TovarName.Length > 35)
                        return BadRequest("Название товара не может превышать 35 символов.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarUnit))
                        return BadRequest("Единица измерения является обязательным полем.");

                    if (tovar.TovarCost <= 0)
                        return BadRequest("Цена товара должна быть положительным числом.");

                    if (tovar.TovarCount <= 0)
                        return BadRequest("Кол-во товара должно быть положительным числом.");

                    if (tovar.TovarCurrentSale < 0 || tovar.TovarCurrentSale > 99)
                        return BadRequest("Скидка должна быть в диапазоне от 0 до 99%.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarDesc))
                        return BadRequest("Описание товара является обязательным полем.");

                    var newTovar = new Tovar
                    {
                        TovarArticle = generatedArticle,
                        TovarName = tovar.TovarName.Trim(),
                        TovarUnit = tovar.TovarUnit.Trim(),
                        TovarDesc = tovar.TovarDesc.Trim(),
                        TovarCost = tovar.TovarCost,
                        TovarCount = tovar.TovarCount,
                        TovarCurrentSale = tovar.TovarCurrentSale,
                        SupplierId = tovar.SupplierId,
                        ManufactrurerId = tovar.ManufactrurerId,
                        TovarCategoryId = tovar.TovarCategoryId,
                        TovarPhoto = tovar.TovarPhoto
                    };

                    context.Tovars.Add(newTovar);
                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Товар успешно добавлен!",
                        article = newTovar.TovarArticle,
                        name = newTovar.TovarName,
                        generatedArticle = true

                    });
                }
                catch (DbUpdateException dbEx)
                {
                    return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpPut("RedactTovar")]
        public ActionResult RedactGood([FromBody] Tovar tovar)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (tovar == null)
                        return BadRequest("Данные товара не предоставлены.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarArticle))
                        return BadRequest("Артикул товара является обязательным полем.");

                    var existingGood = context.Tovars
                        .FirstOrDefault(c => c.TovarArticle == tovar.TovarArticle);

                    if (existingGood == null)
                        return NotFound($"Товар с артикулом '{tovar.TovarArticle}' не найден.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarName))
                        return BadRequest("Название товара является обязательным полем.");

                    if (tovar.TovarName.Length > 35)
                        return BadRequest("Название товара не может превышать 35 символов.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarUnit))
                        return BadRequest("Единица измерения является обязательным полем.");

                    if (tovar.TovarUnit.Length > 5)
                        return BadRequest("Единица измерения не может превышать 5 символов.");

                    if (tovar.TovarCost <= 0)
                        return BadRequest("Цена товара должна быть положительным числом.");

                    if (tovar.TovarCount < 0)
                        return BadRequest("Количество товара не может быть отрицательным.");

                    if (tovar.TovarCurrentSale < 0 || tovar.TovarCurrentSale > 100)
                        return BadRequest("Скидка должна быть в диапазоне от 0 до 100%.");

                    if (string.IsNullOrWhiteSpace(tovar.TovarDesc))
                        return BadRequest("Описание товара является обязательным полем.");

                    existingGood.TovarName = tovar.TovarName.Trim();
                    existingGood.TovarUnit = tovar.TovarUnit.Trim();
                    existingGood.TovarCost = tovar.TovarCost;
                    existingGood.SupplierId = tovar.SupplierId;
                    existingGood.ManufactrurerId = tovar.ManufactrurerId;
                    existingGood.TovarCategoryId = tovar.TovarCategoryId;
                    existingGood.TovarCurrentSale = tovar.TovarCurrentSale;
                    existingGood.TovarCount = tovar.TovarCount;
                    existingGood.TovarDesc = tovar.TovarDesc.Trim();

                    if (!string.IsNullOrEmpty(tovar.TovarPhoto))
                    {
                        if (!string.IsNullOrEmpty(existingGood.TovarPhoto) &&
                            existingGood.TovarPhoto != tovar.TovarPhoto)
                        {
                            DeleteOldImage(existingGood.TovarPhoto);
                        }
                        existingGood.TovarPhoto = tovar.TovarPhoto;
                    }

                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Товар успешно обновлен!",
                        article = existingGood.TovarArticle,
                        name = existingGood.TovarName
                    });
                }
                catch (DbUpdateException dbEx)
                {
                    return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpDelete("DeleteTovar")]
        public ActionResult DeleteTovar(string article)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(article))
                        return BadRequest("Артикул товара не указан.");

                    var existingGood = context.Tovars
                        .FirstOrDefault(c => c.TovarArticle == article);

                    if (existingGood == null)
                        return NotFound($"Товар с артикулом '{article}' не найден.");

                    var isInOrder = context.OrderCompositions
                        .Any(oc => oc.TovarArticle == article);

                    if (isInOrder)
                        return BadRequest($"Товар '{article}' присутствует в заказах и не может быть удален.");

                    if (!string.IsNullOrEmpty(existingGood.TovarPhoto))
                    {
                        DeleteOldImage(existingGood.TovarPhoto);
                    }

                    context.Tovars.Remove(existingGood);
                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Товар успешно удален!",
                        article = article,
                        name = existingGood.TovarName
                    });
                }
                catch (DbUpdateException dbEx)
                {
                    return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpPost("UploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile image, [FromForm] string? article = null)
        {
            try
            {
                if (image == null || image.Length == 0)
                    return BadRequest("Файл не выбран.");

                // Проверка размера файла
                if (image.Length > IMAGE_MAX_SIZE)
                    return BadRequest($"Файл слишком большой. Максимальный размер: {IMAGE_MAX_SIZE / 1024 / 1024} MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(image.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Недопустимый формат файла. Разрешены: JPG, PNG, GIF, WebP.");

                // Создаем временный файл для проверки размеров
                var tempFilePath = Path.GetTempFileName();

                try
                {
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Проверяем размеры изображения
                    using (var img = System.Drawing.Image.FromFile(tempFilePath))
                    {
                        if (img.Width != IMAGE_WIDTH || img.Height != IMAGE_HEIGHT)
                        {
                            System.IO.File.Delete(tempFilePath);
                            return BadRequest($"Изображение должно быть размером {IMAGE_WIDTH}x{IMAGE_HEIGHT} пикселей. Текущий размер: {img.Width}x{img.Height}");
                        }
                    }

                    // Генерируем имя файла
                    string fileName;
                    if (!string.IsNullOrEmpty(article))
                    {
                        var safeArticle = CleanFileName(article);
                        fileName = $"{safeArticle}{fileExtension}";
                    }
                    else
                    {
                        fileName = $"product_{Guid.NewGuid():N}{fileExtension}";
                    }

                    var vueProjectPath = "C:\\Users\\79174\\Downloads\\Telegram Desktop\\vue-parfum-demo";
                    var resourcesFolder = Path.Combine(vueProjectPath, "public", "resources");

                    // Создаем папку если её нет
                    if (!Directory.Exists(resourcesFolder))
                    {
                        Directory.CreateDirectory(resourcesFolder);
                    }

                    var filePath = Path.Combine(resourcesFolder, fileName);

                    // Если файл с таким именем существует, создаем уникальное имя
                    if (System.IO.File.Exists(filePath))
                    {
                        var counter = 1;
                        var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        while (System.IO.File.Exists(filePath))
                        {
                            fileName = $"{nameWithoutExt}_{counter}{fileExtension}";
                            filePath = Path.Combine(resourcesFolder, fileName);
                            counter++;
                        }
                    }

                    // Копируем временный файл в целевой
                    System.IO.File.Copy(tempFilePath, filePath);

                    return Ok(new
                    {
                        success = true,
                        message = $"Изображение успешно загружено. Размер: {IMAGE_WIDTH}x{IMAGE_HEIGHT}",
                        fileName = fileName,
                        width = IMAGE_WIDTH,
                        height = IMAGE_HEIGHT
                    });
                }
                finally
                {
                    // Удаляем временный файл
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        System.IO.File.Delete(tempFilePath);
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(500, $"Ошибка при загрузке изображения: неверный размер картинки");
            }
        }

        private string CleanFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(fileName
                .Where(ch => !invalidChars.Contains(ch))
                .ToArray());

            const int maxLength = 50;
            if (safeName.Length > maxLength)
            {
                safeName = safeName.Substring(0, maxLength);
            }

            return safeName.Trim();
        }

        private void DeleteOldImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            var vueProjectPath = "C:\\Users\\79174\\Downloads\\Telegram Desktop\\vue-parfum-demo";
            var resourcesFolder = Path.Combine(vueProjectPath, "public", "resources");
            var filePath = Path.Combine(resourcesFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        [HttpGet("GetBasketItems")]
        public ActionResult GetBasketItems(int userId)
        {
            using (var context = new DbParfumeContext())
            {
                var basketItems = context.Bascets
                    .Where(b => b.UserId == userId)
                    .Include(b => b.TovarArticleNavigation)
                        .ThenInclude(t => t.Manufactrurer)
                    .Include(b => b.TovarArticleNavigation)
                        .ThenInclude(t => t.Supplier)
                    .Include(b => b.TovarArticleNavigation)
                        .ThenInclude(t => t.TovarCategory)
                    .Select(b => new
                    {
                        b.UserId,
                        b.TovarArticle,
                        Quantity = b.BascetCount,
                        Product = b.TovarArticleNavigation,
                        MaxQuantity = b.TovarArticleNavigation.TovarCount
                    })
                    .ToList();

                // Для отладки - проверим что приходит
                Console.WriteLine($"Найдено элементов: {basketItems.Count}");
                foreach (var item in basketItems)
                {
                    Console.WriteLine($"Article: {item.TovarArticle}, Quantity: {item.Quantity}");
                }

                return Ok(basketItems);
            }
        }

        [HttpPost("AddToBasket")]
        public ActionResult AddToBasket([FromBody] AddToBasketRequest request)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (request == null)
                        return BadRequest("Данные не предоставлены.");

                    // Проверяем существование товара
                    var product = context.Tovars
                        .FirstOrDefault(t => t.TovarArticle == request.TovarArticle);

                    if (product == null)
                        return NotFound("Товар не найден.");

                    // Проверяем наличие на складе
                    if (product.TovarCount <= 0)
                        return BadRequest("Товар отсутствует на складе.");

                    // Проверяем, есть ли уже такой товар в корзине пользователя
                    var existingItem = context.Bascets
                        .FirstOrDefault(b => b.UserId == request.UserId &&
                                            b.TovarArticle == request.TovarArticle);

                    if (existingItem != null)
                    {
                        // Проверяем, не превысит ли новое количество доступное на складе
                        int newQuantity = (int)existingItem.BascetCount + request.Quantity;
                        if (newQuantity > product.TovarCount)
                        {
                            return BadRequest($"Нельзя добавить больше {product.TovarCount} шт. (доступно на складе)");
                        }

                        existingItem.BascetCount = newQuantity;

                        // Для отладки
                        Console.WriteLine($"Обновлено: Article={request.TovarArticle}, NewQuantity={newQuantity}");
                    }
                    else
                    {
                        // Проверяем, не превышает ли запрашиваемое количество доступное на складе
                        if (request.Quantity > product.TovarCount)
                        {
                            return BadRequest($"Нельзя добавить больше {product.TovarCount} шт. (доступно на складе)");
                        }

                        var basketItem = new Bascet
                        {
                            UserId = request.UserId,
                            TovarArticle = request.TovarArticle,
                            BascetCount = request.Quantity
                        };
                        context.Bascets.Add(basketItem);

                        // Для отладки
                        Console.WriteLine($"Добавлено: Article={request.TovarArticle}, Quantity={request.Quantity}");
                    }

                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Товар добавлен в корзину",
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpPut("UpdateBasketQuantity")]
        public ActionResult UpdateBasketQuantity([FromBody] UpdateBasketRequest request)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (request == null)
                        return BadRequest("Данные не предоставлены.");

                    var basketItem = context.Bascets
                        .FirstOrDefault(b => b.UserId == request.UserId &&
                                            b.TovarArticle == request.TovarArticle);

                    if (basketItem == null)
                        return NotFound("Элемент корзины не найден.");

                    var product = context.Tovars
                        .FirstOrDefault(t => t.TovarArticle == basketItem.TovarArticle);

                    if (product == null)
                        return NotFound("Товар не найден.");

                    if (request.Quantity > product.TovarCount)
                    {
                        return BadRequest($"Нельзя установить количество больше {product.TovarCount} шт. (доступно на складе)");
                    }

                    if (request.Quantity <= 0)
                    {
                        // Если количество 0 или меньше, удаляем товар из корзины
                        context.Bascets.Remove(basketItem);
                    }
                    else
                    {
                        basketItem.BascetCount = request.Quantity;
                    }

                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Количество обновлено",
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpDelete("RemoveFromBasket")]
        public ActionResult RemoveFromBasket([FromBody] RemoveFromBasketRequest request)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var basketItem = context.Bascets
                        .FirstOrDefault(b => b.UserId == request.UserId &&
                                            b.TovarArticle == request.TovarArticle);

                    if (basketItem == null)
                        return NotFound("Элемент корзины не найден.");

                    context.Bascets.Remove(basketItem);
                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Товар удален из корзины",
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        [HttpDelete("ClearBasket/{userId}")]
        public ActionResult ClearBasket(int userId)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var basketItems = context.Bascets
                        .Where(b => b.UserId == userId)
                        .ToList();

                    if (basketItems.Any())
                    {
                        context.Bascets.RemoveRange(basketItems);
                        context.SaveChanges();
                    }

                    return Ok(new
                    {
                        message = "Корзина очищена",
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка: {ex.Message}");
                }
            }
        }

        // Модели запросов
        public class AddToBasketRequest
        {
            public int UserId { get; set; }
            public string TovarArticle { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class UpdateBasketRequest
        {
            public int UserId { get; set; }
            public string TovarArticle { get; set; }
            public int Quantity { get; set; }
        }

        public class RemoveFromBasketRequest
        {
            public int UserId { get; set; }
            public string TovarArticle { get; set; }
        }
        // Добавьте эти методы в контроллер TovarController

        [HttpPost("CreateOrder")]
        public ActionResult CreateOrder([FromBody] CreateOrderRequest request)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    if (request == null || request.Items == null || !request.Items.Any())
                        return BadRequest("Нет товаров для оформления заказа");

                    // Проверяем существование пользователя
                    var user = context.Users.FirstOrDefault(u => u.UserId == request.UserId);
                    if (user == null)
                        return NotFound("Пользователь не найден");

                    // Проверяем существование пункта выдачи
                    var pickUpPoint = context.PickUpPoints.FirstOrDefault(p => p.PickUpPointId == request.PickUpPointId);
                    if (pickUpPoint == null)
                        return NotFound("Пункт выдачи не найден");

                    // Проверяем наличие всех товаров и их количество
                    foreach (var item in request.Items)
                    {
                        var product = context.Tovars.FirstOrDefault(t => t.TovarArticle == item.TovarArticle);
                        if (product == null)
                            return BadRequest($"Товар с артикулом {item.TovarArticle} не найден");

                        if (product.TovarCount < item.Quantity)
                            return BadRequest($"Недостаточно товара {product.TovarName} на складе. Доступно: {product.TovarCount}");
                    }

                    // Генерируем уникальный код заказа (случайное число от 100 до 999)
                    Random random = new Random();
                    int orderCode;
                    do
                    {
                        orderCode = random.Next(100, 1000);
                    } while (context.Orders.Any(o => o.OrderCode == orderCode.ToString()));

                    // Создаем заказ
                    var order = new Order
                    {
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderDateDelivery = DateOnly.FromDateTime(DateTime.Now.AddDays(14)), // Доставка через 14 дней
                        PickUpPointId = request.PickUpPointId,
                        UserId = request.UserId,
                        OrderCode = orderCode.ToString(),
                        OrderStatus = "Новый"
                    };

                    context.Orders.Add(order);
                    context.SaveChanges(); // Сохраняем чтобы получить OrderId

                    // Создаем состав заказа и обновляем количество товаров
                    foreach (var item in request.Items)
                    {
                        var product = context.Tovars.First(t => t.TovarArticle == item.TovarArticle);

                        var orderComposition = new OrderComposition
                        {
                            OrderId = order.OrderId,
                            TovarArticle = item.TovarArticle,
                            OrderCompositionCount = item.Quantity
                        };

                        context.OrderCompositions.Add(orderComposition);

                        // Уменьшаем количество товара на складе
                        product.TovarCount -= item.Quantity;
                    }

                    // Очищаем корзину пользователя
                    var basketItems = context.Bascets.Where(b => b.UserId == request.UserId).ToList();
                    context.Bascets.RemoveRange(basketItems);

                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Заказ успешно оформлен",
                        orderId = order.OrderId,
                        orderCode = order.OrderCode,
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка при оформлении заказа: {ex.Message}");
                }
            }
        }
        [HttpGet("GetPickUpPoints")]
        public ActionResult GetPickUpPoints()
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var points = context.PickUpPoints
                        .Select(p => new
                        {
                            p.PickUpPointId,
                            FullAddress = $"{p.PickUpPointIndex}, {p.PickUpPointCity}, ул.{p.PickUpPointStreet}, д.{p.PickUpPointHome}"
                        })
                        .ToList();

                    return Ok(points);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка при загрузке пунктов выдачи: {ex.Message}");
                }
            }
        }

        [HttpGet("GetUserOrders")]
        public ActionResult GetUserOrders(int userId)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var orders = context.Orders
                        .Where(o => o.UserId == userId)
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.Manufactrurer)
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.Supplier)
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.TovarCategory)
                        .Include(o => o.PickUpPoint)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => new
                        {
                            o.OrderId,
                            OrderDate = o.OrderDate,
                            OrderDateDelivery = o.OrderDateDelivery,
                            o.PickUpPointId,
                            PickUpPointAddress = $"{o.PickUpPoint.PickUpPointIndex}, {o.PickUpPoint.PickUpPointCity}, ул.{o.PickUpPoint.PickUpPointStreet}, д.{o.PickUpPoint.PickUpPointHome}",
                            o.UserId,
                            o.OrderCode,
                            o.OrderStatus,
                            Items = o.OrderCompositions.Select(oc => new
                            {
                                oc.OrderCompositionId,
                                oc.TovarArticle,
                                oc.OrderCompositionCount,
                                Product = new
                                {
                                    oc.TovarArticleNavigation.TovarName,
                                    oc.TovarArticleNavigation.TovarCost,
                                    oc.TovarArticleNavigation.TovarCurrentSale,
                                    oc.TovarArticleNavigation.TovarPhoto,
                                    ManufacturerName = oc.TovarArticleNavigation.Manufactrurer != null ? oc.TovarArticleNavigation.Manufactrurer.ManufacturerName : "",
                                    CategoryName = oc.TovarArticleNavigation.TovarCategory != null ? oc.TovarArticleNavigation.TovarCategory.TovarCategoryName : "",
                                    SupplierName = oc.TovarArticleNavigation.Supplier != null ? oc.TovarArticleNavigation.Supplier.SupplierName : ""
                                },
                                TotalPrice = (oc.TovarArticleNavigation.TovarCurrentSale > 0
                                    ? oc.TovarArticleNavigation.TovarCost * (1 - (decimal)oc.TovarArticleNavigation.TovarCurrentSale / 100)
                                    : oc.TovarArticleNavigation.TovarCost) * oc.OrderCompositionCount
                            }).ToList(),
                            TotalSum = o.OrderCompositions.Sum(oc =>
                                (oc.TovarArticleNavigation.TovarCurrentSale > 0
                                    ? oc.TovarArticleNavigation.TovarCost * (1 - (decimal)oc.TovarArticleNavigation.TovarCurrentSale / 100)
                                    : oc.TovarArticleNavigation.TovarCost) * oc.OrderCompositionCount)
                        })
                        .ToList();

                    return Ok(orders);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка при загрузке заказов: {ex.Message}");
                }
            }
        }

        // Модели запросов
        public class CreateOrderRequest
        {
            public int UserId { get; set; }
            public int PickUpPointId { get; set; }
            public List<OrderItemRequest> Items { get; set; }
        }

        public class OrderItemRequest
        {
            public string TovarArticle { get; set; }
            public int Quantity { get; set; }
        }
        [HttpGet("GetAllOrders")]
        public ActionResult GetAllOrders()
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var orders = context.Orders
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.Manufactrurer)
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.Supplier)
                        .Include(o => o.OrderCompositions)
                            .ThenInclude(oc => oc.TovarArticleNavigation)
                                .ThenInclude(t => t.TovarCategory)
                        .Include(o => o.PickUpPoint)
                        .Include(o => o.User)
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => new
                        {
                            o.OrderId,
                            OrderDate = o.OrderDate,
                            OrderDateDelivery = o.OrderDateDelivery,
                            o.PickUpPointId,
                            PickUpPointAddress = $"{o.PickUpPoint.PickUpPointIndex}, {o.PickUpPoint.PickUpPointCity}, ул.{o.PickUpPoint.PickUpPointStreet}, д.{o.PickUpPoint.PickUpPointHome}",
                            o.UserId,
                            UserFull = $"{o.User.UserSurname} {o.User.UserName} {o.User.UserLastname}",
                            
                            o.OrderCode,
                            o.OrderStatus,
                            Items = o.OrderCompositions.Select(oc => new
                            {
                                oc.OrderCompositionId,
                                oc.TovarArticle,
                                oc.OrderCompositionCount,
                                Product = new
                                {
                                    oc.TovarArticleNavigation.TovarName,
                                    oc.TovarArticleNavigation.TovarCost,
                                    oc.TovarArticleNavigation.TovarCurrentSale,
                                    oc.TovarArticleNavigation.TovarPhoto,
                                    ManufacturerName = oc.TovarArticleNavigation.Manufactrurer != null ? oc.TovarArticleNavigation.Manufactrurer.ManufacturerName : "",
                                    CategoryName = oc.TovarArticleNavigation.TovarCategory != null ? oc.TovarArticleNavigation.TovarCategory.TovarCategoryName : "",
                                    SupplierName = oc.TovarArticleNavigation.Supplier != null ? oc.TovarArticleNavigation.Supplier.SupplierName : ""
                                },
                                TotalPrice = (oc.TovarArticleNavigation.TovarCurrentSale > 0
                                    ? oc.TovarArticleNavigation.TovarCost * (1 - (decimal)oc.TovarArticleNavigation.TovarCurrentSale / 100)
                                    : oc.TovarArticleNavigation.TovarCost) * oc.OrderCompositionCount
                            }).ToList(),
                            TotalSum = o.OrderCompositions.Sum(oc =>
                                (oc.TovarArticleNavigation.TovarCurrentSale > 0
                                    ? oc.TovarArticleNavigation.TovarCost * (1 - (decimal)oc.TovarArticleNavigation.TovarCurrentSale / 100)
                                    : oc.TovarArticleNavigation.TovarCost) * oc.OrderCompositionCount)
                        })
                        .ToList();

                    return Ok(orders);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка при загрузке заказов: {ex.Message}");
                }
            }
        }

        [HttpPut("UpdateOrderStatus/{orderId}")]
        public ActionResult UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            using (var context = new DbParfumeContext())
            {
                try
                {
                    var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId);

                    if (order == null)
                        return NotFound("Заказ не найден");

                    // Проверяем, не завершен ли уже заказ
                    if (order.OrderStatus == "Завершен")
                        return BadRequest("Нельзя изменить статус завершенного заказа");

                    order.OrderStatus = request.OrderStatus;
                    context.SaveChanges();

                    return Ok(new
                    {
                        message = "Статус заказа успешно обновлен",
                        success = true
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Ошибка при обновлении статуса: {ex.Message}");
                }
            }
        }

        // Модель для запроса
        public class UpdateOrderStatusRequest
        {
            public string OrderStatus { get; set; }
        }
    }
}