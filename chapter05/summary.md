# Service
kUbernates服务是一种为一组功能相同的pod提供单一不变的接入口的资源，当服务存在时，它的IP地址和端口不会改变，客户端通过IP地址和端口号建立连接，这些连接会被路由到提供该服务的任意一个Pod上。
- 为什么需要service
答：在非Kubernates的环境中，服务之间互相访问是通过配置文件指定精确的IP地址或者主机名来配置，但在Kubernates中，基于以下原因无法这么做：
  * pod是短暂的，它们会随时启动或关闭。
  * kubernates在pod启动前会给已经调度到节点上的pod分配IP地址，客户端无法提前知道提供服务的Pod的IP地址。
  * 水平伸缩意味着多个pod可能会提供相同的服务，而每个pod都有自己的IP地址，客户端不关心提供服务的pod数量。
- Service的标签选择器与ReplicationController 的机制相同。

## 创建服务
### kubectl expose
将资源暴露为新的Kubernetes Service。

指定deployment、service、replica set、replication controller或pod ，并使用该资源的选择器作为指定端口上新服务的选择器。deployment 或 replica set只有当其选择器可转换为service支持的选择器时，即当选择器仅包含matchLabels组件时才会作为暴露新的Service。
```bash
为RC的nginx创建service，并通过Service的80端口转发至容器的8000端口上。

kubectl expose rc nginx --port=80 --target-port=8000
```
### 通过YAML描述文件创建服务
```bash
apiVersion: v1
kind: Service
metadata:
  name: kubia
spec:
  ports:
  - port: 80            //该服务的可用端口
    targetPort: 8080    // 服务将连接转发到的容器端口
  selector:
    app: kubia          // 具有app=kubia的pod都属于该服务


result:
NAME         TYPE           CLUSTER-IP     EXTERNAL-IP      PORT(S)          AGE
kubia        ClusterIP      10.3.245.235   <none>           80/TCP           10s

// CLUSTER-IP 用于集群内部访问的IP地址
```
- 配置服务上的会话亲和性---- sessionAffinity
  * 将特定客户端产生的所有请求每次都指向同一个pod.
  * 两个值ClientIP和None
- 同一个服务暴露多个端口
```bash
kubia-svc-multi-port.yaml
```

- 使用命名的端口
```bash
kubia-svc-named-port.yaml
```

## 服务发现
客户端如何知道服务的IP地址和端口，以便客户端可以访问。

### 通过环境变量发现 -- 通过在容器中运行env列出所有的环境变量
```bash
kubectl exec kubia-ndx7s env

result:
PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
HOSTNAME=kubia-rjh8m
KUBIA_SERVICE_PORT=80
COREDOCKER_SERVICE_HOST=10.3.246.113
COREDOCKER_SERVICE_PORT=8181
COREDOCKER_PORT_8181_TCP_ADDR=10.3.246.113
KUBERNETES_SERVICE_PORT=443
KUBERNETES_PORT_443_TCP=tcp://10.3.240.1:443
KUBERNETES_PORT_443_TCP_PROTO=tcp
KUBIA_SERVICE_HOST=10.3.245.235
KUBIA_PORT_80_TCP_ADDR=10.3.245.235
COREDOCKER_PORT=tcp://10.3.246.113:8181
COREDOCKER_PORT_8181_TCP_PROTO=tcp
KUBERNETES_PORT_443_TCP_ADDR=10.3.240.1
KUBIA_PORT_80_TCP=tcp://10.3.245.235:80
KUBIA_PORT_80_TCP_PORT=80
COREDOCKER_PORT_8181_TCP=tcp://10.3.246.113:8181
COREDOCKER_PORT_8181_TCP_PORT=8181
KUBERNETES_SERVICE_HOST=10.3.240.1
KUBERNETES_SERVICE_PORT_HTTPS=443
KUBERNETES_PORT=tcp://10.3.240.1:443
KUBERNETES_PORT_443_TCP_PORT=443
KUBIA_PORT=tcp://10.3.245.235:80
KUBIA_PORT_80_TCP_PROTO=tcp
NODE_VERSION=10.16.0
YARN_VERSION=1.16.0
HOME=/root
```
### 通过DNS发现服务(how，不理解)
### 通过FQDN连接服务
```bash
kubectl exec kubia-rjh8m bash

cat /etc/resolve.conf

curl http:/k8s-hello-svc.default.svc.cluster.local

curl http:/k8s-hello-svc.default

curl http:/k8s-hello-svc
```
### 无法ping通服务IP的原因
## 连接集群外部的服务
### 服务endpoint
  服务和pod并不是直接相连的，在两者之间有一种资源--Endpoint,该资源用于暴露一个服务的IP和端口的列表，
## 将服务暴露给外部客户端
