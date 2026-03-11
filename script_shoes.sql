CREATE DATABASE  IF NOT EXISTS `db_parfume` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db_parfume`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: db_parfume
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bascet`
--

DROP TABLE IF EXISTS `bascet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bascet` (
  `user_id` int NOT NULL,
  `tovar_article` varchar(6) NOT NULL,
  `bascet_count` int DEFAULT NULL,
  PRIMARY KEY (`user_id`,`tovar_article`),
  KEY `fk_t_article_idx` (`tovar_article`),
  CONSTRAINT `fk_t_article` FOREIGN KEY (`tovar_article`) REFERENCES `tovars` (`tovar_article`),
  CONSTRAINT `fk_user1` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bascet`
--

LOCK TABLES `bascet` WRITE;
/*!40000 ALTER TABLE `bascet` DISABLE KEYS */;
/*!40000 ALTER TABLE `bascet` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manufacturers`
--

DROP TABLE IF EXISTS `manufacturers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manufacturers` (
  `manufacturer_id` int NOT NULL AUTO_INCREMENT,
  `manufacturer_name` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`manufacturer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manufacturers`
--

LOCK TABLES `manufacturers` WRITE;
/*!40000 ALTER TABLE `manufacturers` DISABLE KEYS */;
INSERT INTO `manufacturers` VALUES (1,'Dior'),(2,'Giorgio Armani'),(3,'Chanel'),(4,'Tom Ford'),(5,'Yves Saint Laurent'),(6,'Jo Malone London'),(7,'Gucci'),(8,'Prada'),(9,'Lancôme'),(10,'Hermès'),(11,'Marc Jacobs'),(12,'Versace'),(13,'Viktor&Rolf'),(14,'Givenchy'),(15,'Dolce&Gabbana'),(16,'Jean Paul Gaultier'),(17,'Montblanc'),(18,'Carolina Herrera'),(19,'Issey Miyake'),(20,'Narciso Rodriguez'),(21,'Paco Rabanne'),(22,'Mugler'),(23,'Bvlgari'),(24,'Valentino'),(25,'Hugo Boss'),(26,'Estée Lauder'),(27,'Davidoff');
/*!40000 ALTER TABLE `manufacturers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_composition`
--

DROP TABLE IF EXISTS `order_composition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_composition` (
  `order_composition_id` int NOT NULL AUTO_INCREMENT,
  `order_id` int DEFAULT NULL,
  `tovar_article` varchar(6) DEFAULT NULL,
  `order_composition_count` int DEFAULT NULL,
  PRIMARY KEY (`order_composition_id`),
  KEY `fk_order_idx` (`order_id`),
  KEY `fk_tovar_idx` (`tovar_article`),
  CONSTRAINT `fk_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`),
  CONSTRAINT `fk_tovar` FOREIGN KEY (`tovar_article`) REFERENCES `tovars` (`tovar_article`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_composition`
--

LOCK TABLES `order_composition` WRITE;
/*!40000 ALTER TABLE `order_composition` DISABLE KEYS */;
INSERT INTO `order_composition` VALUES (1,1,'А112Т4',2),(2,2,'H782T5',1),(3,3,'J384T6',10),(4,4,'F572H7',5),(5,5,'А112Т4',2),(6,6,'H782T5',1),(7,7,'J384T6',10),(8,8,'F572H7',5),(9,9,'B320R5',5),(10,10,'S213E3',5),(11,1,'F635R4',2),(12,2,'G783F5',1),(13,3,'D572U8',10),(14,4,'D329H3',4),(15,5,'F635R4',2),(16,6,'G783F5',1),(17,7,'D572U8',10),(18,8,'D329H3',4),(19,9,'G432E4',1),(20,10,'E482R4',5),(21,11,'123521',1),(22,11,'asdfgh',1),(23,11,'B320R5',1),(24,12,'asdfgh',1),(25,13,'B320R5',1),(26,13,'S326R5',1),(27,14,'D572U8',1),(28,14,'FUKTH5',7),(29,14,'L754R4',1),(30,14,'PZRBES',8),(31,15,'qwerty',2),(32,15,'S213E3',6),(33,16,'L754R4',6),(34,16,'P764G4',2),(35,16,'qwerty',6);
/*!40000 ALTER TABLE `order_composition` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `order_id` int NOT NULL AUTO_INCREMENT,
  `order_date` date DEFAULT NULL,
  `order_date_delivery` date DEFAULT NULL,
  `pick-up_point_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `order_code` varchar(4) DEFAULT NULL,
  `order_status` enum('Завершен','Новый') DEFAULT NULL,
  PRIMARY KEY (`order_id`),
  KEY `fk_pick-up_idx` (`pick-up_point_id`),
  KEY `fk_user_idx` (`user_id`),
  CONSTRAINT `fk_pick-up` FOREIGN KEY (`pick-up_point_id`) REFERENCES `pick-up_points` (`pick-up_point_id`),
  CONSTRAINT `fk_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,'2025-02-27','2025-04-20',1,4,'901','Завершен'),(2,'2022-09-28','2025-04-21',11,1,'902','Завершен'),(3,'2025-03-21','2025-04-22',2,2,'903','Завершен'),(4,'2025-02-20','2025-04-23',11,3,'904','Завершен'),(5,'2025-03-17','2025-04-24',2,4,'905','Завершен'),(6,'2025-03-01','2025-04-25',15,1,'906','Завершен'),(7,'2025-03-01','2025-04-26',3,2,'907','Завершен'),(8,'2025-03-31','2025-04-27',19,3,'908','Завершен'),(9,'2025-04-02','2025-04-28',5,4,'909','Новый'),(10,'2025-04-03','2025-04-29',19,4,'910','Новый'),(11,'2026-02-24','2026-03-10',14,7,'668','Новый'),(12,'2026-02-25','2026-03-11',13,7,'340','Завершен'),(13,'2026-02-25','2026-03-11',13,7,'102','Новый'),(14,'2026-02-26','2026-03-12',4,7,'175','Завершен'),(15,'2026-02-26','2026-03-12',7,7,'180','Новый'),(16,'2026-02-28','2026-03-14',6,7,'855','Новый');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pick-up_points`
--

DROP TABLE IF EXISTS `pick-up_points`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pick-up_points` (
  `pick-up_point_id` int NOT NULL AUTO_INCREMENT,
  `pick-up_point_index` varchar(6) DEFAULT NULL,
  `pick-up_point_city` varchar(20) DEFAULT NULL,
  `pick-up_point_street` varchar(50) DEFAULT NULL,
  `pick-up_point_home` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`pick-up_point_id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pick-up_points`
--

LOCK TABLES `pick-up_points` WRITE;
/*!40000 ALTER TABLE `pick-up_points` DISABLE KEYS */;
INSERT INTO `pick-up_points` VALUES (1,'420151','г. Уфа','ул. Вишневая','32'),(2,'125061','г. Уфа','ул. Подгорная','8'),(3,'630370','г. Уфа','ул. Шоссейная','24'),(4,'400562','г. Уфа','ул. Зеленая','32'),(5,'614510','г. Уфа','ул. Маяковского','47'),(6,'410542','г. Уфа','ул. Светлая','46'),(7,'620839','г. Уфа','ул. Цветочная','8'),(8,'443890','г. Уфа','ул. Коммунистическая','1'),(9,'603379','г. Уфа','ул. Спортивная','46'),(10,'603721','г. Уфа','ул. Гоголя','41'),(11,'410172','г. Уфа','ул. Северная','13'),(12,'614611','г. Уфа','ул. Молодежная','50'),(13,'454311','г. Уфа','ул. Новая','19'),(14,'660007','г. Уфа','ул. Октябрьская','19'),(15,'603036','г. Уфа','ул. Садовая','4'),(16,'394060','г. Уфа','ул. Фрунзе','43'),(17,'410661','г. Уфа','ул. Школьная','50'),(18,'625590','г. Уфа','ул. Коммунистическая','20'),(19,'625683','г. Уфа','ул. 8 Марта',''),(20,'450983','г. Уфа','ул. Комсомольская','26'),(21,'394782','г. Уфа','ул. Чехова','3'),(22,'603002','г. Уфа','ул. Дзержинского','28'),(23,'450558','г. Уфа','ул. Набережная','30'),(24,'344288','г. Уфа','ул. Чехова','1'),(25,'614164','г. Уфа','  ул. Степная','30'),(26,'394242','г. Уфа','ул. Коммунистическая','43'),(27,'660540','г. Уфа','ул. Солнечная','25'),(28,'125837','г. Уфа','ул. Шоссейная','40'),(29,'125703','г. Уфа','ул. Партизанская','49'),(30,'625283','г. Уфа','ул. Победы','46'),(31,'614753','г. Уфа','ул. Полевая','35'),(32,'426030','г. Уфа','ул. Маяковского','44'),(33,'450375','г. Уфа','ул. Клубная','44'),(34,'625560','г. Уфа','ул. Некрасова','12'),(35,'630201','г. Уфа','ул. Комсомольская','17'),(36,'190949','г. Уфа','ул. Мичурина','26');
/*!40000 ALTER TABLE `pick-up_points` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `supplier_name` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'Luxe & Tradition'),(2,'Trend & Volume');
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovar_category`
--

DROP TABLE IF EXISTS `tovar_category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovar_category` (
  `tovar_category_id` int NOT NULL AUTO_INCREMENT,
  `tovar_category_name` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`tovar_category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovar_category`
--

LOCK TABLES `tovar_category` WRITE;
/*!40000 ALTER TABLE `tovar_category` DISABLE KEYS */;
INSERT INTO `tovar_category` VALUES (1,'Женский парфюм'),(2,'Мужской парфюм');
/*!40000 ALTER TABLE `tovar_category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovars`
--

DROP TABLE IF EXISTS `tovars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovars` (
  `tovar_article` varchar(6) NOT NULL,
  `tovar_name` varchar(35) DEFAULT NULL,
  `tovar_unit` varchar(4) DEFAULT NULL,
  `tovar_cost` decimal(10,2) DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `manufactrurer_id` int DEFAULT NULL,
  `tovar_category_id` int DEFAULT NULL,
  `tovar_current_sale` int DEFAULT NULL,
  `tovar_count` int DEFAULT NULL,
  `tovar_desc` text,
  `tovar_photo` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`tovar_article`),
  KEY `fk_supplier_idx` (`supplier_id`),
  KEY `fk_manufacturer_idx` (`manufactrurer_id`),
  KEY `fk_category_idx` (`tovar_category_id`),
  CONSTRAINT `fk_category` FOREIGN KEY (`tovar_category_id`) REFERENCES `tovar_category` (`tovar_category_id`),
  CONSTRAINT `fk_manufacturer` FOREIGN KEY (`manufactrurer_id`) REFERENCES `manufacturers` (`manufacturer_id`),
  CONSTRAINT `fk_supplier` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovars`
--

LOCK TABLES `tovars` WRITE;
/*!40000 ALTER TABLE `tovars` DISABLE KEYS */;
INSERT INTO `tovars` VALUES ('123521','йцукенг','шт.',7878.00,2,15,1,12,0,'оченб ароматный парфюм','product_214811f888294d47b8d19ad63debade4.png'),('8J6LZK','df','шт.',454576.00,1,15,1,0,0,'df','product_63be923d69754d90a440bd2ddc01bad9.jpg'),('asdfgh','Versace Bright crystal','шт.',13200.00,2,12,1,25,58,'Свежая и соблазнительная композиция BRIGHT CRYSTAL PARFUM – это вдохновляющая симфония цветочных нот, дополненных яркими фруктовыми акцентами.','product_e455da0dfb81436b9178e93ceb41f29c.webp'),('B320R5','Dior Sauvage','шт.',7900.00,2,1,2,2,4,'Мужской парфюм Dior Sauvage Elixir, 60 мл: интенсивный, пряный древесный аромат с нотами ладана и мускуса','6.webp'),('BPOFZ5','qwer','шт.',566.67,1,4,2,45,6,'qwert','product_0145f799b8f54bdcafe3391c0e810326.jpg'),('C436G5','Мисс Диор (Miss Dior)','шт.',10200.00,1,1,1,15,9,'Женский парфюм Miss Dior Blooming Bouquet: цветочный, нежный аромат с нотами бергамота и розы',NULL),('C4GIJ1','ugbtg','шт.',78.00,1,17,1,1,7,'gty',''),('D268G5','Valentino Voce Viva','шт.',4399.00,2,24,1,3,12,'Женский парфюм Valentino Voce Viva Intensa, 100 мл: смелый, амброво-цветочный аромат с мандарином, туберозой и ванилью',NULL),('D329H3','Davidoff Cool Water','шт.',4600.00,1,27,2,4,4,'Мужской парфюм Davidoff Cool Water Wave, 125 мл: освежающий, водный аромат с дыней, мятой и мускусом','5.webp'),('D364R4','Narciso Rodriguez For Her','шт.',12400.00,1,20,1,16,5,'Женский парфюм Narciso Rodriguez For Her Musc Noir Rose, 90 мл: загадочный, цветочно-мускусный аромат с розой и касаи',NULL),('D572U8','Carolina Herrera Good Girl','шт.',10200.00,2,18,1,3,5,'Женский парфюм Carolina Herrera Good Girl Blush, 80 мл: нежный, цветочно-мускусный аромат с жасмином и лепестками розы','2.webp'),('E482R4','Givenchy Gentleman','шт.',1800.00,2,14,2,2,14,'Мужской парфюм Givenchy Gentleman Boisée, 100 мл: элегантный, бархатистый древесный аромат с кардамоном и пачули',NULL),('F427R5','Montblanc Explorer','шт.',11800.00,1,17,2,15,11,'Мужской парфюм Montblanc Explorer Ultra Blue, 100 мл: динамичный, водный аромат с мандарином, базиликом и амброй',NULL),('F572H7','Chanel Coco Mademoiselle','шт.',17890.00,2,3,1,2,14,'Женский парфюм Chanel Coco Mademoiselle, интенсивная версия, 50 мл: чувственный восточный аромат с пачули и ветивером',NULL),('F635R4','Armani Si','шт.',12960.00,1,2,1,2,13,'Женский парфюм Giorgio Armani Si Passione Eclat, 100 мл: яркий, фруктово-мускусный аромат с грушей и ванилью','3.webp'),('FUKTH5','tytytty','шт.',89.00,1,16,1,1,0,'fgfyv','product_73fed36fabe94c88ba053b42a847d37e.png'),('G432E4','Dolce&Gabbana Light Blue','шт.',2800.00,1,15,1,3,15,'Женский парфюм Dolce&Gabbana Light Blue Forever, 50 мл: сочный, цитрусовый аромат с нотой грейпфрута и цветков яблони',NULL),('G531F4','Jean Paul Gaultier Le Male','шт.',6600.00,2,16,2,12,9,'Мужской парфюм Jean Paul Gaultier Le Male Le Parfum, 125 мл: чувственный, ванильно-древесный аромат с ирисом и кардамоном',NULL),('G783F5','Boss Bottled','шт.',10080.00,1,25,2,2,8,'Мужской парфюм Hugo Boss Boss Bottled Elixir, 100 мл: насыщенный, пряно-яблочный аромат с имбирем и кедром','4.webp'),('H535R5','Black Opium','шт.',2300.00,1,5,1,2,7,'Женский парфюм Yves Saint Laurent Black Opium, 90 мл: навязчивый, сладкий кофейный аромат с аккордами ванили и белых цветов',NULL),('H782T5','Bleu de Chanel','шт.',12800.00,2,3,2,4,5,'Мужской парфюм Chanel Bleu de Chanel Parfum, 100 мл: утонченный, амброво-древесный аромат с нотами цитрусов и имбиря',NULL),('J384T6','Bvlgari Man','шт.',9300.00,1,23,2,2,16,'Мужской парфюм Bvlgari Man in Black, 100 мл: натуральный, землистый аромат с ветивером, грейпфрутом и перцем','1.webp'),('J542F5','Jo Malone London','шт.',500.00,1,6,1,13,0,'Женский парфюм Jo Malone London Wood Sage & Sea Salt, 30 мл: свежий, минеральный аромат морского бриза с шалфеем и амброй',NULL),('K358H6','Viktor&Rolf Flowerbomb','шт.',599.00,1,13,1,20,2,'Женский парфюм Viktor&Rolf Flowerbomb Ruby Orchid, 90 мл: пудрово-сладкий цветочный аромат с орхидеей и гранатом',NULL),('L754R4','Prada L\'Homme','шт.',1700.00,2,8,2,2,0,'Мужской парфюм Prada L\'Homme Intense, 100 мл: элегантный, пудрово-древесный аромат с ирисом и сандалом','product_f2f74e8954f049dcb3f10fb3ace4c650.jpg'),('LR41M2','духиии','шт.',3243.00,1,15,1,1,340,'qwer',''),('M542T5','Tom Ford Oud Wood','шт.',2800.00,1,4,2,18,3,'Мужской парфюм Tom Ford Oud Wood, 50 мл: роскошный, теплый древесный аромат с сердцем из сандала и ветивера',NULL),('N457T5','Mugler Alien','шт.',4600.00,2,22,1,3,13,'Женский парфюм Mugler Angel Nova, 50 мл: ягодно-древесный, магнетический аромат с малиной, личи и пачули',NULL),('NMB9LW','sfdhfjgj','шт.',435.00,1,17,1,1,5,'fsgdhf',''),('O754F4','Issey Miyake L\'Eau d\'Issey','шт.',5400.00,2,19,2,4,18,'Мужской парфюм Issey Miyake L\'Eau d\'Issey Pour Homme Intense, 125 мл: глубокий, водно-древесный аромат с иланг-илангом и мускатным орехом',NULL),('P764G4','Marc Jacobs Daisy','шт.',6800.00,2,11,1,15,13,'Женский парфюм Marc Jacobs Daisy Dream Eau de Toilette, 100 мл: воздушный, ягодно-цветочный аромат с черной смородиной и голубым гиацинтом',NULL),('PZRBES','ytigty','шт.',78.00,1,2,1,8,0,'ytiy','product_1cc1a95f690b47c99385669aa54bc161.jpg'),('qwerty','духи','шт.',2000.00,1,14,1,14,82,'йцукен','product_b92660a50c8743bd98252b2dd4040ab7.jpg'),('S213E3','Estée Lauder Beautiful','шт.',2156.00,1,26,1,3,0,'Женский парфюм Estée Lauder Beautiful Magnolia, 100 мл: романтичный, цветочный букет с магнолией, пионом и фрезией',NULL),('S326R5','Paco Rabanne 1 Million','шт.',9900.00,1,21,2,17,14,'Мужской парфюм Paco Rabanne 1 Million Lucky, 100 мл: теплый, орехово-фруктовый аромат с лесным орехом и сливой',NULL),('S634B5','Gucci Bloom','шт.',5500.00,1,7,1,3,0,'Женский парфюм Gucci Bloom Acqua di Fiori, 100 мл: весенний, зеленый цветочный аромат с жасмином и гиацинтом',NULL),('А112Т4','Acqua di Giò','шт.',4567.00,2,2,2,3,0,'Мужской парфюм Giorgio Armani Acqua di Giò Profondo, 100 мл: свежий морской аромат с нотами цитрусовых и лаванды',NULL);
/*!40000 ALTER TABLE `tovars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `user_role` varchar(50) DEFAULT NULL,
  `user_surname` varchar(30) DEFAULT NULL,
  `user_name` varchar(25) DEFAULT NULL,
  `user_lastname` varchar(30) DEFAULT NULL,
  `user_login` varchar(55) DEFAULT NULL,
  `user_password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Администратор','Никифорова','Весения','Николаевна','94d5ous@gmail.com','uzWC67'),(2,'Администратор','Сазонов','Руслан','Германович','uth4iz@mail.com','2L6KZG'),(3,'Администратор','Одинцов','Серафим','Артёмович','yzls62@outlook.com','JlFRCZ'),(4,'Менеджер','Степанов','Михаил','Артёмович','1diph5e@tutanota.com','8ntwUp'),(5,'Менеджер','Ворсин','Петр','Евгеньевич','tjde7c@yahoo.com','YOyhfR'),(6,'Менеджер','Старикова','Елена','Павловна','wpmrc3do@tutanota.com','RSbvHv'),(7,'Авторизированный клиент','Михайлюк','Анна','Вячеславовна','5d4zbu@tutanota.com','rwVDh9'),(8,'Авторизированный клиент','Ситдикова','Елена','Анатольевна','ptec8ym@yahoo.com','LdNyos'),(9,'Авторизированный клиент','Ворсин','Петр','Евгеньевич','1qz4kw@mail.com','gynQMT'),(10,'Авторизированный клиент','Старикова','Елена','Павловна','4np6se@mail.com','AtnDjr'),(11,'Авторизированный клиент','Бадамшина','Аделина','Ришатовна','adelya1@gmail.com','Adelya123');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-11 18:01:06
