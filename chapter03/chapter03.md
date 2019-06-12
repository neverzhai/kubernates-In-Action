# Pod
## node VS Pod VS docker container
 node 是集群中的节点， 一个node上可以有多个pod。
 pod是运行并管理容器的逻辑主机，一个pod上可以有多个容器。
 - 如何查看应用运行在哪个节点上
   * kubectl get pods -o wide
   * kubectl describe pod <pod-name> 
### 通过pod合理管理容器
- 将多层应用分散到多个pod中
- 基于扩缩容考虑而分割到多个pod中。
- 何时在pod中使用多个容器
  * 它们需要一起运行还是可以在不同的主机上运行。
  * 它们代表一个主体还是不同的组件。
  * 它们必须一起进行扩缩容还是可以分别进行。
## 创建pod
- 以yaml或JSON描述文件创建pod.
  * pod 定义的主要部分。
    * metada: 包括名称。命名空间，标签和关于改容器的其他信息。
    * spec： 包含pod内容的实际说明，例如pod的容器，卷和其他数据。
    * status：包含运行中的pod的当前信息，例如所处的条件，每个容器的描述和状态，以及内部IP和其他信息。
  * 可以通过kubectl explain命令查看可能的API字段。
   * kubectl explain pods
   * kubectl explain pod.spec
## 查看应用程序日志
- 使用kubectl logs 命令获取pod日志
```yaml
kubectl logs <pod-name>
```
- 获取多容器的pod的日志时指定容器名称
```yaml
kubectl logs <pod-name> -c <container-name>
```
## 向pod发送请求
- 将本地端口转发到pod中的端口
```yaml
kubectl port-forward <pod-name> 8888(local-port):8080(pod-port)
```
# labels
## labels的定义及作用
标签是可以附加到资源的任意键值对，用以选择具有改确切标签（标签选择器）的资源。
标签主要用于对资源进行分类管理，比如指定一个pod是属于哪个应用，哪个为服务（前端 or 后端）。
## 标签的创建
- 创建pod时指点标签（pod-with-label.yaml）
- 修改现有标签
```yaml
kubectl label po <po-name> <label-key>=<label-name>
```
- 显示标签
```yaml
kubectl get po --show-labels

kubectl get po -L <label1-key>,<label2-key>
```
## 标签的使用
- 通过标签选择器列出pod子集
```yaml
kubectl get po -l <label-key>=<label-value>
kubectl get po -l <label-key>
kubectl get po -l '!<label-key>'
kubectl get po -l <label-key>!=<label-value>
kubectl get po -l <label-key> in (<label-value1>, <lable-value-2>)
kubectl get po -l <label-key> notin (<label-value1>, <lable-value-2>)
```
- 使用标签选择器来约束pod调度
使用标签对node进行分类，然后可以将pod调度到特定的节点上（见yaml文件）
# 注解（annotate）干啥用的
# namespace
kubenetes的命名空间简单的为对象名称提供一个作用域，在同一个命名空间下资源名称不同相同，但不同命名空间下可以相同。
- kubectl get ns
- kubectl get po --namespace/-n <name> 
## 创建一个命名空间
- yaml文件
```yaml
  apiVersion: v1
  kind: Namespace
  metadata: 
  name: custom-namespace
```
- kubectl create namespace命令
  ```yaml
   kubectl create namespace custom-namespace
  ```
## 管理其他命名空间中的对象
- 运行命令时通过 --namespace/-n 指定命名空间
- 通过config切换命名空间
## 命名空间提供的隔离
- 命名空间不提供正在运行的对象的任何隔离，命名空间之间是否提供网络隔离取决于Kubernetes所使用的网络解决方案。
# 停止和移除pod
### 按名移除
kubectl delete po <name>
### 使用标签选择器删除pod
kubectl delete po -l <label-key>=<label-value>
### 删除整个命名空间
kubectl delete ns <namespace-name>
### 删除命名空间的所有pod,但是保留命名空间
kubectl delete pod --all
### 删除命名空间几乎所有资源
kubectl delete all --all