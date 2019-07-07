为了能让开发人员不需要知道底层使用了哪种存储技术，提供给了两种新的资源持久卷和持久卷声明，由运维人员来定义持久卷，由开发人员提供持久卷声明，指定所需要的最低容量需要、访问模式，并将持久卷声明提交到Kubernates API服务器，服务器将找到可匹配的持久卷，并将其绑定到持久卷声明。
# PersistentVolume(PV)
## 创建持久卷
创建持久卷时，管理员需要告诉Kubernates 以下内容：
- 对应的容量需求，以及它是否可以由单个节点或多个节点同时读取和写入，
- 如何处理PV（当PVC的绑定被删除时）
- 指定持久卷支持的实际存储类型，位置以及其他属性。
example参见：mongodb-pv-gecpd.yaml
## 列举持久卷
 ```
  kuberctl get pv
 ```
# PersistentVolumeClaim（PVC）
## 创建持久卷声明
当创建好声明，kubernates就会找到适当的持久卷并将其绑定到声明。
## 列举持久卷声明
```
kubectl get pvc
```
## 持久卷声明的访问模式简写
- RWO： ReadWriteOnce,  仅允许单个节点挂载读写。
- ROX：ReadOnlyMany, 允许多个节点挂载只读。
- RWX：ReadWriteMany，允许多个节点挂载读写这个卷。
## 在pod中使用持久卷声明
example参见： mongodb-pod-pvc.yaml
## 使用持久卷和持久卷声明的好处
- 虽然需要额外的步骤来创建持久卷和持久卷声明，但是研发人员不需要关心底层实际使用的存储技术。
- 可以在许多不同的kubernates集群上使用相同的pod和持久卷声明清单。
## 回收持久卷
设置persistentVplumeReclaimPolicy
### 手动回收
将上面的值设置为Retain,让kubernates在持久卷从持久卷声明中释放时仍能保留他的卷和数据内容。
如果想手动回收持久卷，使其恢复可用状态，只能删除和重新创建持久卷。
### 自动回收
存在两种可回收粗略Recycle和Delete。
Recycle会删除卷的内容，并使卷可用于再次声明。
Delete策略删除底层存储，
# StorageClass（SC）
## 持久卷的动态配置
有了持久卷和持久卷声明后，依旧需要运维人员来管理持久卷，以满足开发人员对于存储的需要。
Kubernates 服务可以通过自动配置持久卷来自动执行此任务。
集群管理人员可以创建一个持久卷配置，并定义一个或多个StorageClass对象，用户可以在持久卷声明中引用StorageClass，而配置程序在配置持久存储时将采用这一点。
### 置备程序--用于决定提供什么样的持久卷
Kubernates包括最流行的云服务提供商的置备程序Provisioner，但如果是部署在本地，则需要配置定制的置备程序。
### 通过StorageClass资源定义可用存储类型
StorageClass资源指定当持久卷声明请求此StorageClass时使用哪个置备程序提供持久卷。
example参见： storageclass-fast-gcepd.yaml
### 在持久卷声明中指定存储类
example参见：mongodb-pvc-dp.yaml
### 不指定存储类的动态配置
example参见：mongdb-pvc-dp-nostorageclass.yaml

此处有个注意事项，当将storageClassName属性设置为空字符串时，可确保PVC绑定到预先配置的PV，而不是动态配置新的PV。当为配置为空字符串，则尽管已经存储适当的预配置持久卷，但动态卷配置仍将配置新的持久卷。
