## Kubernetes Downward API和configMap的应用场景有什么不同？
configMap是向应用程序传递配置信息，且这些信息都是提前知道的，但是对于那些不能提前知道的数据，如pod的IP， 主机名，pod自身的名称，可以通过Downward API来传递pod的元数据。Downward API提供了两种方式，环境变量和文件的方式。

## Kubernetes Downward API都可以对外暴露pod的哪些数据？
- pod名称
- pod 的IP
- Pod所在的命名空间
- Pod运行节点的名称
- pod运行所归属的服务账户的名称
- 每个容器请求的CPU和内存的使用量
- 每个容器可以使用的CPU和内存的限制
- pod的标签
- pod的注解

## 通过环境变量暴露pod的名称，IP，容器请求的CPU（yaml文件）

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: downward
spec:
  containers:
  - name: main
    image: busybox
    command: ["sleep", "9999999"]
    resources:
      requests:
        cpu: 15m
        memory: 100Ki
      limits:
        cpu: 100m
        memory: 4Mi
    env:
    - name: POD_NAME
      valueFrom:
        fieldRef:
          fieldPath: metadata.name
    - name: POD_NAMESPACE
      valueFrom:
        fieldRef:
          fieldPath: metadata.namespac
    - name: CONTAINER_CPU_REQUEST_MILLICORES
      valueFrom:
        resourceFieldRef:
          resource: requests.cpu
          divisor: 1m
```

## 通过DownwardAPI卷暴露pod的名称，标签，容器请求的CPU（yaml文件）
```yaml
volumes:
  - name: downward
    downwardAPI:
      items:
      - path: "podName"
        fieldRef:
          fieldPath: metadata.name
      - path: "labels"
        fieldRef:
          fieldPath: metadata.labels
      - path: "containerCpuRequestMilliCores"
        resourceFieldRef:
          containerName: main
          resource: requests.cpu
          divisor: 1m
```
## 为什么pod的标签和注解只能通过API卷的方式暴露
P236，因为我们可以在pod运行时修改标签和注解，当标签和注解被修改后，kubernates会更新存有相关信息的文件，从而使Pod获取最新的数据，在环境变量方式下，一旦值被修改，新的值无法被暴露。

## 通过API卷的方式暴露容器级别的元数据时，需要注意什么问题？
 必须指定资源字段对应的容器名称，因为我们对于卷的定义是pod级别的，而不是容器级别的，因此当我们通过卷定义某个容器资源字段时，要明确说明引用的容器名称。对于只包含单个容器的资源也同样适用。

## 在本机如何与Kubenetes API服务器交互
通过Downward API的方式获取的元数据是非常有限的，如果想获得更多的元数据，则需要使用直接访问kubernates API服务器的方式。

因为服务器使用的是HTTPS协议，所以要访问Kubernates API, 则必须经过服务器的证书检查和身份认证。

为了避免上述验证环节，可以通过kubectl proxy 命令来访问。
 

## 如果想要从pod内部与Kubernetes API服务器进行交互，需要关注哪些事？
P242
- 确定API服务器的位置
- 确保是与API服务器进行交互，而不是一个冒名者。
- 通过服务器的认证，否则将不能查看任何内容及进行任何操作。

## 如何获得Kubernetes API服务器的IP和端口
- 发现服务器地址
  一个名为kubernates的服务在默认的命名空间被自动暴露，并配置为指向API服务器。
通过命令`kubectl get svc`。
- 验证服务器身份
 P244 在讨论Secret时，提到一个default-token-xyz的secret会被自动创建，并挂载在/var/run/secrets/kubernates.io/serviceaccount 下，其中的ca.crt包含了CA证书。（A证书是指啥）

- 获得API服务器授权
P245 使用default-token-secret来产生凭证


## 简要说明pod与Kubernetes API服务器交互的过程（操作pod所在命名空间的资源）
 P247
 - 应用应该验证API服务器的证书是否是证书机构所签发，这个证书是在ca.crt中。
 - 应用应该将它在token文件中持有的凭证通过Authorization标头来获得API服务器的授权。
 - 当对pod所在的命名空间的API对象进行CRUD时，应该使用namespace文件来传递命名空间信息到API服务器。

## ambassador容器模式用来解决什么问题？有什么缺点
 ambassador容器模式是指在运行主容器的同时，启动一个ambassador容器，并在其中运行kubectl proxy命令，通过它来事项与API服务器的交互。
 优点：将加密、授权，服务器验证工作交给ambassador容器中的kubectl proxy
 缺点：需要运行额外的进程，并且消耗资源。

## 如果需要执行复杂的Kubernetes API操作，可以使用什么方式实现？
使用一个标准的kubenates API客户端库会更好一点。
- 官方版：
  * Golang client: 
  * python 
- 非官方版：
  * Fabric8维护的Java客户端
  * Amdatu维护的Java客户端
  * tenxcloud维护的Node.js客户端
  * GoDaddy护的Node.js客户端
  * ....