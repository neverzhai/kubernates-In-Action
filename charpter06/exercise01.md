1. Volume、PersistentVolume、PersistentVolumeClaim、StorageClass的scope分别是什么
Volume是pod的一个组成部分，并和pod共享相同的生命周期，即pod在启动时创建卷，在删除pod时销毁卷。其Scope时pod.
PersistentVolume，StorageClass都是Kubernates的一种资源，scope是kuberates集群 
pvc 是namespace scope。

2.假如需要部署node-exporter来收集节点指标数据，需要怎样配置volume
 使用hostPath
 Volumes:
   - name:
   hostPath: 

3.假如某个应用需要从secret中加载证书文件，需要怎样配置volume？？？

Volumes:
  Secret: 

4.手动创建一个pv，会发生什么。
   如果pv存在，则创建是是available的，否则是不available的

5.创建一个带storageClass的pvc，会发生什么
   如果不配置会怎样：
   如果配置会怎样：

6.创建pvc时不带storageClassName和storageClassName:””，有区别吗
答： 有区别，如果不带storageClassName,则尽管已经存在适当的预配置持久卷，但动态卷置备程序仍将配置新的持久卷。 
如果带有“”，则可将PVC绑定到预先配置的PV。

7. reclaimPolicy的作用
答：配置持久卷的回收方式
有三个值： Retain， Recycle， Delete， 需要注意的是使用什么回收策略需要底层的存储技术支持。
- Retain： 在持久卷从持久卷声明中释放后仍能保留它的卷和数据内容，这种情况下，需要手动的回收持久卷，即删除和重新创建持久卷资源。
- Recycle：删除卷并使卷可以用于再次声明。
- Delete： 删除底层存储。