# MMORPG项目简介
此项目为练习项目，代码写的可能不是很好，很多细节也没有考虑到，仅供参考！

## SERVER - C# 服务器项目

使用C#网络api从0搭建框架，服务器和客户端之间使用protobuf进行通信，使用Mysql存储数据。

使用excel表格配置地图数据、人物属性、地图上怪物分布情况、掉落物属性等，然后转换为json数据给服务器和客户端解析。

使用aoi算法优化实体之间的交互、检测等逻辑，以及减少同步的网络带宽占用。

### Common类库

此库即为服务器和客户端之间共用的类库

- Data文件夹内存储的就是excel表格，以及对应转换的json和生成的cs文件
- Inventory文件夹为背包的一些通用代码
- Network为通用网络框架
- Proto为protobuf和生成的cs代码
- Tool为一些通用的工具api

## MMORPG - Unity客户端项目

#### 使用QFramework框架，MVC架构

没了XD

## 项目构建和运行

//TODO 还没写XD

## 注意

Unity客户端里面的资源禁止商用！！！
