# 卷（Volume）
- Volume是pod的一个组成部分，并和pod共享相同的生命周期，即pod在启动时创建卷，在删除pod时销毁卷。其Scope时pod.
- pod中的所有容器都可以使用卷，但是必须先将它挂载到每个需要访问他的容器中。
## 卷的类型：：https://kubernetes.io/docs/concepts/storage/volumes/#types-of-volumes
- emptyDir：用于存储临时数据的简单空目录
- hostPath：用于将目录从工作节点的文件系统挂载到pod中。
- gitRepo：通过检出Git仓库的内容来初始化卷。
- nfs: 挂载到pod中的NFS共享卷。
- 用于挂载云服务商的特定存储类型，如gcePersistentDisk，awsElasticBloockStore， azuerDisk。
- 用于挂载其他网络类型的网络存储
- 用于将k8s部分资源和集群信息公开给pod的特殊类型的卷。
- persistentVolumeClaim： 一种使用预置或者动态配置的持久存储类型。
### hostPath
对于某些系统级别的pod需要读取节点的文件或者使用节点文件系统来访问节点设备，k8s通过hostPath卷来实现这一点。它指向节点文件系统上的特定文件或目录。
### 使用持久化存储
如果使用的是云服务器提供商提供的存储可以使用特定的存储类型。如gecpd，见gcepd-pod-volume.yaml
如果使用的是自有服务器，可以使用NFS卷，见nfs-pod-volume.yaml

### 缺点
将这种涉及基础设施类型的信息塞到一个pod设置中，意味着pod设置与特定k8s集群有很大耦合。为了将pod从底层存储技术解藕出来，k8s引入持久卷和持久卷声明的概念，将存储变成一种资源，创建pod时根据需要申请即可。

