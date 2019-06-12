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


