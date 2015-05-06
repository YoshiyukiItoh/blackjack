[oracle@node1 ~]$ sqlplus / as sysdba

SQL*Plus: Release 11.2.0.1.0 Production on 水 5月 6 23:27:02 2015

Copyright (c) 1982, 2009, Oracle.  All rights reserved.



Oracle Database 11g Enterprise Edition Release 11.2.0.1.0 - 64bit Production
With the Partitioning, Real Application Clusters, Automatic Storage Management, OLAP,
Data Mining and Real Application Testing options
に接続されました。
SQL>
SQL>
SQL> select * from v$version;

BANNER
--------------------------------------------------------------------------------
Oracle Database 11g Enterprise Edition Release 11.2.0.1.0 - 64bit Production
PL/SQL Release 11.2.0.1.0 - Production
CORE    11.2.0.1.0      Production
TNS for Linux: Version 11.2.0.1.0 - Production
NLSRTL Version 11.2.0.1.0 - Production

SQL>
SQL>
SQL>
SQL> create user test
  2  identified by test;

ユーザーが作成されました。

SQL> grant connect to test;

権限付与が成功しました。

SQL> grant resource to test;

権限付与が成功しました。

SQL> conn test/test
接続されました。
SQL>
SQL> create table player_info (
  2  name varchar2(10),
  3  win_count number default 0,
  4  primary key (name)
  5  );

表が作成されました。

SQL>
SQL> select * from tab;

TNAME
--------------------------------------------------------------------------------
TABTYPE                CLUSTERID
--------------------- ----------
PLAYER_INFO
TABLE

SQL> desc player_info;
 名前                                    NULL?    型
 ----------------------------------------- -------- ----------------------------
 NAME                                      NOT NULL VARCHAR2(10)
 WIN_COUNT                                          NUMBER

SQL>