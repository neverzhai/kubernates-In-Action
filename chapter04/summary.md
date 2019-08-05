# 存活探针
存活探针主要用于检查容器是否还在运行。
## 三种探针机制
- HTTP GET探针对容器的IP地址（指定端口和路径）执行HTTP GET请求。(pod-with-http-liveness-probe.yaml)
- TCP套接字探针尝试与容器指定端口建立TCP连接。
- EXEC探针，在容器内执行任意命令，并检查命令的退出状态码。

## 存活探针的使用
- 使用yaml文件建立一个pod
  ```bash
  kubectl create -f pod-with-http-liveness-probe.yaml
  ```
- 查看容器重启状态
  * kubectl get pod 
  ```bash
  kubectl get po pod-with-http-liveness-probe

  result: RESTARTS 列显示了重启的次数
  ```
  * kubectl describe pod 
  ```bash
   kubectl describe po pod-with-http-liveness-probe

   result: Restart count 显示重启的次数
           ExitCode： 137
  ```
 ## 配置存活探针的附加属性 
 从上面的kubectl describe  pod 命令中会得到如下探针的附加信息：
 Liveness:     http-get http://:8080/ delay=0s timeout=1s period=10s #success=1 #failure=3
 delay=0s：表示在容器重启后立即开始探测, **创建是一定要设置**
 timeout=1s: 表示容器必须在1s内进行响应，否则本次探测记录为失败。
 period=10s: 表示每10s探测一次容器。
 failure=3: 表示连续三次失败后重启容器。
- 可以在创建pod是指定探针的附加属性。
## 存活探针应该检查什么
最简单的存活探针应该检查服务器是否响应，但为了更好的进行检测，可以设置检查特定的请求，如health check 请求，以方便对应用程序内部的所有重要组件进行状态检查。
另外，在检查时一定要检查应用程序的内部，而不能有任何外部因素的影响。
最好保证存活探针轻量，不要消耗太多的计算资源和时间。

# ReplicationController
ReplicationController 是一种kubernetes资源，用于确保pod始终保持运行状态，如果发现pod消失，可以创建替代的pod（仅针对托管的pod）,如果有多余的pod,则会删除.
- ReolicationController 通过标签选择器是否匹配来调整pod的数量。
- RepicationController 的三部分
  * label selector （标签选择器）
  * replica count（副本个数）
  * pod template (pod 模板)
- 使用RepliationController的好处
  * 确保一个pod(或多个副本)持续运行。
  * 集群节点发生故障时，它将为节点上运行的所有pod创建替代副本。
  * 能实现pod的水平伸缩--手动or自动。
## 创建和使用ReplicationController
```bash
kubectl create -f kubia-replication-controller.yaml
``` 
 创建rc后会先创建副本数量的pod， 可以通过kubectl get po -l app=kubia 查看。
- 针对pod被删除的机制
 当手动删除一个pod后再次查看会看到一个pod已经被终止，并且创建一个新的。
```bash
result:
NAME          READY   STATUS    RESTARTS   AGE
kubia-g5snf   1/1     Running   0          1m
kubia-gnxp4   1/1     Running   0          1m
kubia-t4rml   1/1     Running   0          1m

result after delete:
kubia-7z7cb   1/1     Running   0          36s
kubia-gnxp4   1/1     Running   0          4m
kubia-t4rml   1/1     Running   0          4m

```
- 针对节点的故障
 手动将一个节点断开网络连接
 ```bash
 kubectl get po -o wide // 获取node name
 gcloud compute ssh <node-name>

 sudo ifconfig eth0 down // 断开网络连接

 kubectl get po -l app=kubia
 result: 
  kubia-7z7cb   1/1     Unknown   0          11m  //故障节点上的pod的状态时unknown
  kubia-gnxp4   1/1     Running   0          15m
  kubia-kwch5   1/1     Running   0          55s
  kubia-t4rml   1/1     Running   0          15m

 ```
- 获取rc的信息
```bash
kubectl get rc

kubectl describe rc kubia
```
## 将pod移入或移除RC作用域
由于ReplicationController是通过标签选择器进行匹配，所以可以根据修改标签来完成移入和移除的操作。

## 修改RC的yaml文件
通常用于修改pod模板和副本数量。
```bash
kubectl edit rc kubia
```
## 删除一个RepliationController
```bash
kubectl delete rc kubia --cascade=false

// 指定--cascade=false 保证删除rc时，pod继续运行。 但这些pod不会再被管理。 
```

# ReplicaSet 
ReplicaSet是新一代的ReplicationController, 最终要完全替换掉ReplicationController。
- ReplicaSet功能与ReplicationController完全相同，但具有更加强大的标签选择器。具体体现在ReplictionController的标签选择器只允许包含某个标签的匹配pod.但是ReplicaSet可以匹配缺少某个标签的pod，包含特定标签的pod,匹配多个标签的pod等。
RS和RC只能管理的yaml文件只能配置一个pod模版, 也可以不配置模版，它们会管理空间内label能够匹配的pod.
## 创建ReplicaSet
```bash 
kubectl create -f kubia-replicaSet.yaml
```
## 更强大的标签选择器表达式
 ```bash
 matchExpresss:
      - key: app
        operator: In
        values:
          - kubia
 ```
 其中Operator分为以下几种类型
 - In: Label 值必须与其中一个指定的values值匹配。
 - NotIn: Label 值必须与任何指定的values值不匹配。
 - Exists: pod必须包含一个指定名称的标签（值不需要匹配）， 不应该指定values字段。
 - DoesNotExist: pod不得包含指定名称的标签。

 # DaemonSet
 - DaemonSet的作用是在每个节点上运行一个pod, 也可以只在特定的节点上运行pod.
 - DaemonSet与RC，RS不同的是，它不是保证有多少个pod副本在运行，而是保证匹配标签选择器的节点上都运行了一个pod.

```bash
kubectl create -f ssd-monitor-daemonset.yaml
```

# 执行单个任务的Pod
Kubernates通过Job资源完成只想运行完成工作后就终止任务的情况。
- 创建一个Job运行一个pod
```bash
kubectl create -f batch-job.yaml
```
- 顺序运行Job pod -- completions
```batch
kubectl create -f multi-completion-batch-job.yaml
```
- 并行Job pod
```batch
kubectl create -f multi-completion-parallel-batch-job.yaml
```
- 限制Job pod完成任务时间--activeDeadlineSeconds
   * 如果pod运行时间超过次时间，系统将尝试终止pod,并将job标记为失败。
- Cron Job--安排job定期运行或在将来运行一次
```batch
kubectl create -f cronjob.yaml
```
- 指定截止时间----startingDeadlineSeconds
```bash
startingDeadlineSeconds: 15
// 任务必须在预定时间后15内运行，否则任务将不会运行，并将显示为Failed。
```
