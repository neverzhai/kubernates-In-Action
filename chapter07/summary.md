# ConfigMap 和Secret：配置应用程序
几乎所有的应用程序都需要配置信息（不同部署实例间的区分设置，访问外部系统的证书等），且这些配置数据不应该嵌入应用本身，那kubernates是如何将这些信息传递给应用程序的呢？
 ## 配置容器化应用程序
 - 配置应用程序的方法
   * 向容器传递命令行参数
   * 为每个容器设置自定义的环境变量
   * 通过特殊类型的卷将配置文件挂载到容器中。
### 向容器中传递命令行参数
- 在Docker中定义命令和参数
 * 容器中完整指令由两部分组成：命令和参数，
 * Dockerfile中有两种指令分别定义命令和参数： ENTRYPOINT和CMD
   * ENTRYPOINT： 指定容器启动时被调用的可执行程序
   * CMD： 指定传递给ENTRYPIOINT的参数
 * 了解shell和exec形式的区别
   * 两者的区别是指定的命令运行是否在shell中被调用。shell进程往往是多余的，因此一般直接采用exec形式的ENTRYPOINT指令。
- 在Kubernates中覆盖命令行参数
  * 在kubernates 中定义容器时，镜像中的ENTRYPOINT和CMD都可以被覆盖。对应的命令时command和args. 通常情况下只需要自定义参数，命令一般很少覆盖
  ```bash
  kind: Pod
  spec:
    containers:
    - image: some/image
      command: ["bin/command"]
      args:["arg1","arg2"]
  ```
### 为容器设置环境变量
- 在容器定义中指定环境变量
  * Kubernates允许为pod中的每一个容器都指定自定义的环境变量集合，因此环境变量被设置在pod级的，而不是pod级。
  * 环境变量列表无法在pod创建后被修改。
```yaml
kind: Pod
spec:
  containers:
  - image: some/image
  env:
  - name: INTERNAL
    value: 2
```
- 在环境变量中引用其他环境变量
  * 可以使用$(VAR) 语法在环境变量中引用其他环境变量
  ```yaml
  kind: Pod
spec:
  containers:
  - image: some/image
  env:
  - name: INTERNAL
    value: 2
  - name: SECOND_INTERNAL
    value: "$(INTERNAL)3"
  ```
- 了解硬编码环境变量的不足之处
  * pod定义硬编码意味着需要有效区分生产环境和开发过程中的pod的定义，为了能在多个环境下复用pod的定义，需要将配置从pod定义描述中解耦出来，ConfigMap资源就是解决这个问题的。
### 使用ConfigMap解耦配置
- ConfigMap介绍
  * kubernates允许将配置选项分离到单独的资源对象ConfigMap中，本质上是键/值映射，值可以是短字面量，也可以是完整的配置文件。
  * 映射的内容通过环境变量或者是卷文件传递给容器。
  * 命令行参数的定义可以通过$(ENV_VAR)语法引用环境变量，因而可以达到将ConfigMap的条目当作命令行参数传递给进程的效果。
  * pod通过名称应用configMap
- 创建ConfigMap
  * 利用kubectl创建ConfigMap
  ```bash
  kubectl create configmap fortune-config --from-literal=sleep-interval=25

  ConfigMap: fortune-config
  sleep-interval : 25
  ```
  * 通过kubernates API 创建对应的ConfigMap
  ```
  kubectl create -f fortune-config.yaml
  ```
  * 从文件内容创建ConfigMap
   将文件内容单独存储为configMap中的条目
  ```
  kubectl create configmap my-config --from-file=config-file.conf
   // config-file.conf 为键， 文件内容为值

  kubectl create configmap my-config --from-file=customkey=config-file.conf
   // 手动指定customkey 为键，文件内容为值
  ```
  * 从文件夹创建ConfigMap
  为文件夹中分每一个条目创建条目
  ```
  kubectl create configmap my-config --from-file=/path/to/dir
  ```
  * 合并不同选项
  创建configMap时可以混合使用前面提到的所有项。
### 将映射中的值传递给容易有三种方法
- 给容器传递ConfigMap条目作为环境变量
 ```
 kind: Pod
 metadata:
  name: fortune-env-from-configmap
 spec:
  containers:
  - image: luksa/fortune:env
    env:
    - name: INTERVAL
      valueFrom: 
        configMapKeyRef:
          name: fortune-config  // configmap 的名称
          key: sleep-interval   //键
 ```
   * 在pod中引用不存在的configMap会发生什么？
     kubernates会正常调度pod并尝试运行所有的容器，引用不存在的configMap的容器会启动失败，其余容器会正常启动。 如果之后创建了这个缺失的configMap,失败的容器会自动启动。

- 一次性传递ConfigMap所有条目作为环境变量
```
kind: Pod
 metadata:
  name: fortune-env-from-configmap
 spec:
  containers:
  - image: luksa/fortune:env
    envFrom:
    - prefix: CONFIG_
      configMapKeyRef:
        name: fortune-config  // configmap 的名称 
                              // 将所有条目都变成环境变量
```
- 传递ConfigMap条目作为命令行参数
```
kind: Pod
 metadata:
  name: fortune-env-from-configmap
 spec:
  containers:
  - image: luksa/fortune:env
    env:
    - name: INTERVAL
    valueFrom:
      configMapKeyRef:
        name: fortune-config  // configmap 的名称 
        key: sleep-internal
   args: ["$(INTERVAL)"]     // INTERVAL 作为命令行参数
```
- 使用ConfigMap卷将条目暴露为文件
  * 环境变量或者命令行参数值作为配置通常适用于变量值较短的场景，由于configMap中可以包含完整的配置文件的内容，当你想要将其暴露给容器时，可以借助ConfigMap卷的特殊卷格式。
  （ConfigMap卷是啥，和configMap有什么不同）
  * 创建一个pod,并在卷内使用configMap的条目
  * 在使用configMap卷时，可以指定ConfigMap中的条目:items
  * 可以为configMap卷中的文件设置权限： defaultMode
  ```
   volumes:
  - name: html
    emptyDir: {}
  - name: config                      //卷定义引用fortune-config configMap
    configMap:
      name: fortune-config
      items:                          //  选择包含在卷中的条目
      - key: my-nginx-config.conf
        path: gzip.conf
      defaultMode: "6600"             // 设置文件的权限

  ``` 
- 更新应用配置且不用重启应用程序

### 使用Secret给容器传递敏感数据
- 介绍Secret
  Kubernates提供了一种叫做Secret的资源来给容器传递敏感信息。传递的方式跟ConfigMap是一样的，需要注意的是Secret只会存储在节点的内存中，不会写入物理存储（如果节点死了会怎样？）
- 默认令牌Secret介绍
  * Kubernates提供了一个会挂载在所有容器的Secret。使用如下命令可以查看：
  ```
  kubectl describe pog <pod-name> 
  ```
   * 这个Secret中包含了从pod内部访问Kubernates API 服务器所需要的全部信息
   ```
   kubectl get secrtes

   kubectl describe secret <secret-name>

   // result 
Name:         default-token-kqz7q
Namespace:    default
Labels:       <none>
Annotations:  kubernetes.io/service-account.name: default
              kubernetes.io/service-account.uid: de1a99cb-4fc3-11e9-b1cf-42010a8001f4

Type:  kubernetes.io/service-account-token

Data
====
ca.crt:     1115 bytes
namespace:  7 bytes
token:      eyJhbGciOiJSUzI1NiIsImtpZCI6IiJ9.eyJpc3MiOiJrdWJlcm5ldGVzL3NlcnZpY2VhY2NvdW50Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9uYW1lc3BhY2UiOiJkZWZhdWx0Iiwia3ViZXJuZXRlcy5pby9zZXJ2aWNlYWNjb3VudC9zZWNyZXQubmFtZSI6ImRlZmF1bHQtdG9rZW4ta3F6N3EiLCJrdWJlcm5ldGVzLmlvL3NlcnZpY2VhY2NvdW50L3NlcnZpY2UtYWNjb3VudC5uYW1lIjoiZGVmYXVsdCIsImt1YmVybmV0ZXMuaW8vc2VydmljZWFjY291bnQvc2VydmljZS1hY2NvdW50LnVpZCI6ImRlMWE5OWNiLTRmYzMtMTFlOS1iMWNmLTQyMDEwYTgwMDFmNCIsInN1YiI6InN5c3RlbTpzZXJ2aWNlYWNjb3VudDpkZWZhdWx0OmRlZmF1bHQifQ.Z3EQ9sA9mmn05qgRb5qOyRj-S8BPVnHqZhHOJfIQh2RSeP78HjIyOOlaakLH7MDrzURyh807SSurYta9Df1hUnqFanCXi4WlkTG_Yw3xEoplNs9T0y1qmktaJWougD3WfnekXjoeeE8QUnt7uO8KPZk0XMS5OGA2WvEf1cilOsl3Wsq03bDXVV4dHrqjjUOJSxX8m0gZS5D4eeGu_8rh7fpet8uNvTejLo3BJgX-qD52QGaEysa19PNieqHJsF2qaZDZkMc43bTI2qD_6AHuwhtEesDJV_IOejqb2S-lxg-rfhdQO_YgUCCZeVFVZW1ya9GXxarm6CVjPxSXlzguXQ
   ```
  从上述结果中可以看出这个默认的Secret中包含了三个条目（在data中显示）
    * ca.crt
    * namespace
    * token
  * 使用kubectl describe pod 可以查看默认Secret被挂载的位置
- 创建Secret
  * 创建方式同ConfigMap
  ```
  kubectl create secret generic fortune-https --from-file=https.key --from-file=https.cert --from-file=foo
  ```
  * Secret 有多少中类型？？？？
- 对比ConfigMap和Secret
  * Secret条目内容会被以Base64格式编码，而ConfigMap直接以纯文本展示。
  * 可以通过StringData来设置条目为纯文本的值。（使用create命令时如何指定StringData???）
  * Secret的大小限制为1MB。
- 在pod中使用Secret
  ```
  参见 fortune-config-https.yaml

  step2: create configmap
  kubectl create configmap fortune-config --from-flie=configmap-files

  step2: create secret
  kubectl create secret generic fortune-https --from-file=https.key --from-file=https.cert --from-file=foo

  step3: create pod
  kubectl create -f fortune-config-https.yaml

  step4: port forward 
   kubectl port-forward fortune-https 8443:443 &

  step5: call 8443
  curl https://localhost:8443 -k -v
  ```

