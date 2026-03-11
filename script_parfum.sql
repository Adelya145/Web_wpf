CREATE DATABASE  IF NOT EXISTS `db_shoes` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db_shoes`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: localhost    Database: db_shoes
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
-- Table structure for table `manufacturers`
--

DROP TABLE IF EXISTS `manufacturers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manufacturers` (
  `manufacturer_id` int NOT NULL AUTO_INCREMENT,
  `manufacturer_name` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`manufacturer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manufacturers`
--

LOCK TABLES `manufacturers` WRITE;
/*!40000 ALTER TABLE `manufacturers` DISABLE KEYS */;
INSERT INTO `manufacturers` VALUES (1,'Alessio Nesca'),(2,'CROSBY'),(3,'Kari'),(4,'Marco Tozzi'),(5,'Rieker'),(6,'Рос');
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
  KEY `fk_tovar_idx` (`tovar_article`),
  KEY `fk_order_idx` (`order_id`),
  CONSTRAINT `fk_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`),
  CONSTRAINT `fk_tovar` FOREIGN KEY (`tovar_article`) REFERENCES `tovars` (`tovar_article`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_composition`
--

LOCK TABLES `order_composition` WRITE;
/*!40000 ALTER TABLE `order_composition` DISABLE KEYS */;
INSERT INTO `order_composition` VALUES (1,1,'А112Т4',2),(2,2,'H782T5',1),(3,3,'J384T6',10),(4,4,'F572H7',5),(5,5,'А112Т4',2),(6,6,'H782T5',1),(7,7,'J384T6',10),(8,8,'F572H7',5),(9,9,'B320R5',5),(10,10,'S213E3',5),(11,1,'F635R4',2),(12,2,'G783F5',1),(13,3,'D572U8',10),(14,4,'D329H3',4),(15,5,'F635R4',2),(16,6,'G783F5',1),(17,7,'D572U8',10),(18,8,'D329H3',4),(19,9,'G432E4',1),(20,10,'E482R4',5),(24,11,'C436G5',1),(26,12,'J384T6',1),(33,15,'870700',1),(34,13,'870700',3);
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
  `order_code` varchar(3) DEFAULT NULL,
  `order_status` enum('Завершен','Новый') DEFAULT NULL,
  PRIMARY KEY (`order_id`),
  KEY `fk_puck-up_idx` (`pick-up_point_id`),
  KEY `fk_user_idx` (`user_id`),
  CONSTRAINT `fk_puck-up` FOREIGN KEY (`pick-up_point_id`) REFERENCES `pick-up_points` (`pick-up_point_id`),
  CONSTRAINT `fk_user` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,'2025-02-27','2025-04-20',9,4,'901','Новый'),(2,'2022-09-28','2025-04-21',11,1,'902','Завершен'),(3,'2025-03-21','2025-04-22',2,2,'903','Завершен'),(4,'2025-02-20','2025-04-23',11,3,'904','Завершен'),(5,'2025-03-17','2025-04-24',2,4,'905','Завершен'),(6,'2025-03-01','2025-04-25',15,1,'906','Завершен'),(7,'2025-03-01','2025-04-26',3,2,'907','Завершен'),(8,'2025-03-31','2025-04-27',19,3,'908','Новый'),(9,'2025-04-02','2025-04-28',5,4,'909','Новый'),(10,'2025-04-18','2025-04-29',23,5,'910','Новый'),(11,'2026-02-18','2026-02-21',3,2,'411','Новый'),(12,'2026-02-20','2026-03-01',1,4,'795','Завершен'),(13,'2026-02-21','2026-02-28',8,3,'916','Завершен'),(15,'2026-03-03','2026-03-07',2,1,'945','Новый');
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
INSERT INTO `pick-up_points` VALUES (1,'420151','г. Лесной','ул. Вишневая','32'),(2,'125061','г. Лесной','ул. Подгорная','8'),(3,'630370','г. Лесной','ул. Шоссейная','24'),(4,'400562','г. Лесной','ул. Зеленая','32'),(5,'614510','г. Лесной','ул. Маяковского','47'),(6,'410542','г. Лесной','ул. Светлая','46'),(7,'620839','г. Лесной','ул. Цветочная','8'),(8,'443890','г. Лесной','ул. Коммунистическая','1'),(9,'603379','г. Лесной','ул. Спортивная','46'),(10,'603721','г. Лесной','ул. Гоголя','41'),(11,'410172','г. Лесной','ул. Северная','13'),(12,'614611','г. Лесной','ул. Молодежная','50'),(13,'454311','г.Лесной','ул. Новая','19'),(14,'660007','г.Лесной','ул. Октябрьская','19'),(15,'603036','г. Лесной','ул. Садовая','4'),(16,'394060','г.Лесной','ул. Фрунзе','43'),(17,'410661','г. Лесной','ул. Школьная','50'),(18,'625590','г. Лесной','ул. Коммунистическая','20'),(19,'625683','г. Лесной','ул. 8 Марта',''),(20,'450983','г.Лесной','ул. Комсомольская','26'),(21,'394782','г. Лесной','ул. Чехова','3'),(22,'603002','г. Лесной','ул. Дзержинского','28'),(23,'450558','г. Лесной','ул. Набережная','30'),(24,'344288','г. Лесной','ул. Чехова','1'),(25,'614164','г.Лесной','  ул. Степная','30'),(26,'394242','г. Лесной','ул. Коммунистическая','43'),(27,'660540','г. Лесной','ул. Солнечная','25'),(28,'125837','г. Лесной','ул. Шоссейная','40'),(29,'125703','г. Лесной','ул. Партизанская','49'),(30,'625283','г. Лесной','ул. Победы','46'),(31,'614753','г. Лесной','ул. Полевая','35'),(32,'426030','г. Лесной','ул. Маяковского','44'),(33,'450375','г. Лесной','ул. Клубная','44'),(34,'625560','г. Лесной','ул. Некрасова','12'),(35,'630201','г. Лесной','ул. Комсомольская','17'),(36,'190949','г. Лесной','ул. Мичурина','26');
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
INSERT INTO `suppliers` VALUES (1,'Kari'),(2,'Обувь для вас');
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
INSERT INTO `tovar_category` VALUES (1,'Женская обувь'),(2,'Мужская обувь');
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
  `tovar_photo` varchar(40) DEFAULT NULL,
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
INSERT INTO `tovars` VALUES ('0QSI9W','Туфли','шт.',13608.00,2,5,1,12,34,'Современные туфли 2026 года','picture.png'),('700708','7ggvy','yu',878.00,2,1,2,78,78,'vhgvhgjv','img_20260219_121705_263_f68a836e.jpg'),('789867','пнпг','нг',787787.12,1,1,2,78,78,'прп','img_20260219_120824_400_40a8a1b0.jpg'),('870700','пррл','пр',65885.00,2,1,2,67,67,'аппоа','img_20260219_114251_186_ee6a631a.jpg'),('897008','прорп','пр',8787.00,1,1,2,78,78,'гро','img_20260219_002926_366_ed23b220.png'),('987657','cfgf','yu',877.00,1,2,2,78,78,'gkg','img_20260219_000850_092_9a63dc68.jpg'),('asdfgh','Сандали','шт.',2300.00,2,5,1,6,45,'Самые лучшие сандали','asdfgh.jpg'),('ATOENZ','шщощгшошщ','шт.',898.00,1,1,1,1,1,'ингрг','picture.png'),('B320R5','Туфли','шт.',4500.00,2,5,1,2,6,'Туфли Rieker женские демисезонные, размер 41, цвет коричневый','9.jpg'),('B431R5','Ботинки','шт.',2700.00,1,5,2,2,5,'Мужские кожаные ботинки/мужские ботинки',NULL),('C436G5','Ботинки','шт.',10200.00,1,1,1,15,9,'Ботинки женские, ARGO, размер 40',NULL),('D268G5','Туфли','шт.',4399.00,2,5,1,3,12,'Туфли Rieker женские демисезонные, размер 36, цвет коричневый',NULL),('D329H3','Полуботинки','шт.',1890.00,1,1,1,4,4,'Полуботинки Alessio Nesca женские 3-30797-47, размер 37, цвет: бордовый','8.jpg'),('D364R4','Туфли','шт.',12400.00,1,3,1,16,5,'Туфли Luiza Belly женские Kate-lazo черные из натуральной замши',NULL),('D572U8','Кроссовки','шт.',4100.00,2,6,2,3,6,'129615-4 Кроссовки мужские','6.jpg'),('E482R4','Полуботинки','шт.',1800.00,2,3,1,2,14,'Полуботинки kari женские MYZ20S-149, размер 41, цвет: черный',NULL),('ertyjk','hniuh','ui',8788.00,1,2,1,78,78,'hgvygg','img_20260219_000135_956_9f4015f0.png'),('F427R5','Ботинки','шт.',11800.00,1,5,1,15,11,'Ботинки на молнии с декоративной пряжкой FRAU',NULL),('F572H7','Туфли','шт.',2700.00,2,4,1,2,14,'Туфли Marco Tozzi женские летние, размер 39, цвет черный','7.jpg'),('F635R4','Ботинки','шт.',3244.00,1,4,1,2,13,'Ботинки Marco Tozzi женские демисезонные, размер 39, цвет бежевый','2.jpg'),('G432E4','Туфли','шт.',2800.00,1,3,1,3,15,'Туфли kari женские TR-YR-413017, размер 37, цвет: черный','10.jpg'),('G531F4','Ботинки','шт.',6600.00,2,3,1,12,9,'Ботинки женские зимние ROMER арт. 893167-01 Черный',NULL),('G783F5','Ботинки','шт.',5900.00,1,6,2,2,8,'Мужские ботинки Рос-Обувь кожаные с натуральным мехом','4.jpg'),('ghjggg','hgkuygyu','hj',7000.00,2,1,2,7,0,'ghg','ghjggg.png'),('H535R5','Ботинки','шт.',2300.00,1,5,1,2,7,'Женские Ботинки демисезонные',NULL),('H782T5','Туфли','шт.',4499.00,2,3,2,4,5,'Туфли kari мужские классика MYZ21AW-450A, размер 43, цвет: черный','3.jpg'),('IG8IGC','Кроссовки','шт.',6500.00,1,3,2,90,10,'Кроссовки мечты','img_20260301_173208_850_5b1dfaf3.jpg'),('J384T6','Ботинки','шт.',3800.00,1,5,2,2,16,'B3430/14 Полуботинки мужские Rieker','5.jpg'),('J542F5','Тапочки','шт.',500.00,1,3,2,13,0,'Тапочки мужские Арт.70701-55-67син р.41',NULL),('K345R4','Полуботинки','шт.',2100.00,2,2,2,2,3,'407700/01-02 Полуботинки мужские CROSBY',NULL),('K358H6','Тапочки','шт.',599.00,1,5,2,20,2,'Тапочки мужские син р.41',NULL),('L754R4','Полуботинки','шт.',1700.00,2,3,1,2,7,'Полуботинки kari женские WB2020SS-26, размер 38, цвет: черный',NULL),('M542T5','Кроссовки','шт.',2800.00,1,5,2,18,3,'Кроссовки мужские TOFA',NULL),('N457T5','Полуботинки','шт.',4600.00,2,2,1,3,13,'Полуботинки Ботинки черные зимние, мех',NULL),('O754F4','Туфли','шт.',5400.00,2,5,1,4,18,'Туфли женские демисезонные Rieker артикул 55073-68/37',NULL),('P764G4','Туфли','шт.',6800.00,2,2,1,15,15,'Туфли женские, ARGO, размер 38',NULL),('qwerty','qwerty','l',5000.00,2,4,2,23,0,'qwertytrdf ierter iero','qwerty.jpg'),('S213E3','Полуботинки','шт.',2156.00,1,2,2,3,6,'407700/01-01 Полуботинки мужские CROSBY',NULL),('S326R5','Тапочки','шт.',9900.00,1,2,2,17,15,'Мужские кожаные тапочки \"Профиль С.Дали\" ',NULL),('S634B5','Кеды','шт.',5500.00,1,2,2,3,0,'Кеды Caprice мужские демисезонные, размер 42, цвет черный',NULL),('T324F5','Сапоги','шт.',4699.00,2,2,1,2,5,'Сапоги замша Цвет: синий',NULL),('Z6MY3I','Туфли','шт.',13608.00,2,5,1,12,34,'Современные туфли 2026 года','picture.png'),('А112Т4','Ботинки','шт.',4990.00,2,3,1,3,6,'Женские Ботинки демисезонные kari','1.jpg');
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
  `user_role` varchar(30) DEFAULT NULL,
  `user_surname` varchar(30) DEFAULT NULL,
  `user_name` varchar(25) DEFAULT NULL,
  `user_lastname` varchar(30) DEFAULT NULL,
  `user_login` varchar(60) DEFAULT NULL,
  `user_password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Администратор','Никифорова','Весения','Николаевна','94d5ous@gmail.com','uzWC67'),(2,'Администратор','Сазонов','Руслан','Германович','uth4iz@mail.com','2L6KZG'),(3,'Администратор','Одинцов','Серафим','Артёмович','yzls62@outlook.com','JlFRCZ'),(4,'Менеджер','Степанов','Михаил','Артёмович','1diph5e@tutanota.com','8ntwUp'),(5,'Менеджер','Ворсин','Петр','Евгеньевич','tjde7c@yahoo.com','YOyhfR'),(6,'Менеджер','Старикова','Елена','Павловна','wpmrc3do@tutanota.com','RSbvHv'),(7,'Авторизированный клиент','Михайлюк','Анна','Вячеславовна','5d4zbu@tutanota.com','rwVDh9'),(8,'Авторизированный клиент','Ситдикова','Елена','Анатольевна','ptec8ym@yahoo.com','LdNyos'),(9,'Авторизированный клиент','Ворсин','Петр','Евгеньевич','1qz4kw@mail.com','gynQMT'),(10,'Авторизированный клиент','Старикова','Елена','Павловна','4np6se@mail.com','AtnDjr');
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

-- Dump completed on 2026-03-11 18:01:46
