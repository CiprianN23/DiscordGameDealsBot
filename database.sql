-- MariaDB dump 10.19  Distrib 10.5.12-MariaDB, for Win64 (AMD64)
--
-- Host: localhost    Database: dealsbot
-- ------------------------------------------------------
-- Server version	10.5.12-MariaDB

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `discord_channels`
--

DROP TABLE IF EXISTS `discord_channels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `discord_channels` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `guildid` bigint(20) unsigned NOT NULL,
  `channelid` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UC_DiscordChannels` (`guildid`),
  CONSTRAINT `FK_Guilds_Channels` FOREIGN KEY (`guildid`) REFERENCES `discord_guilds` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `discord_channels`
--

LOCK TABLES `discord_channels` WRITE;
/*!40000 ALTER TABLE `discord_channels` DISABLE KEYS */;
/*!40000 ALTER TABLE `discord_channels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `discord_guilds`
--

DROP TABLE IF EXISTS `discord_guilds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `discord_guilds` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `guild` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UC_Guilds` (`guild`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `discord_guilds`
--

LOCK TABLES `discord_guilds` WRITE;
/*!40000 ALTER TABLE `discord_guilds` DISABLE KEYS */;
/*!40000 ALTER TABLE `discord_guilds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `discord_messages`
--

DROP TABLE IF EXISTS `discord_messages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `discord_messages` (
  `messageid` bigint(20) unsigned NOT NULL,
  `redditpost` bigint(20) unsigned NOT NULL,
  `channelid` bigint(20) unsigned NOT NULL,
  KEY `FK_Messages_Reddit` (`redditpost`),
  KEY `FK_Messages_Channel` (`channelid`),
  CONSTRAINT `FK_Messages_Channel` FOREIGN KEY (`channelid`) REFERENCES `discord_channels` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Messages_Reddit` FOREIGN KEY (`redditpost`) REFERENCES `reddit_posts` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `discord_messages`
--

LOCK TABLES `discord_messages` WRITE;
/*!40000 ALTER TABLE `discord_messages` DISABLE KEYS */;
/*!40000 ALTER TABLE `discord_messages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reddit_posts`
--

DROP TABLE IF EXISTS `reddit_posts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reddit_posts` (
  `id` bigint(20) unsigned NOT NULL AUTO_INCREMENT,
  `fullname` varchar(20) NOT NULL,
  `permalink` varchar(120) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reddit_posts`
--

LOCK TABLES `reddit_posts` WRITE;
/*!40000 ALTER TABLE `reddit_posts` DISABLE KEYS */;
/*!40000 ALTER TABLE `reddit_posts` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-09-17 15:50:59
