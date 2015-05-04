-----------
-- login
-----------
$ mysql -uroot -p

-----------
-- db 作成
-----------
mysql> create database test character set utf8;

mysql> show databases;
+--------------------+
| Database           |
+--------------------+
| information_schema |
| mysql              |
| performance_schema |
| test               |
+--------------------+
4 rows in set (0.10 sec)

mysql> use test;
Database changed
mysql> show tables;
Empty set (0.00 sec)

-----------
-- table 作成
-----------
mysql> create table player_info (
    -> name varchar(10),
    -> win_count int default 0,
    -> primary key (name)
    -> );
Query OK, 0 rows affected (0.07 sec)

mysql> show tables;
+----------------+
| Tables_in_test |
+----------------+
| player_info    |
+----------------+
1 row in set (0.00 sec)

mysql> desc player_info
    -> ;
+-----------+-------------+------+-----+---------+-------+
| Field     | Type        | Null | Key | Default | Extra |
+-----------+-------------+------+-----+---------+-------+
| name      | varchar(10) | NO   | PRI |         |       |
| win_count | int(11)     | YES  |     | 0       |       |
+-----------+-------------+------+-----+---------+-------+
2 rows in set (0.11 sec)
