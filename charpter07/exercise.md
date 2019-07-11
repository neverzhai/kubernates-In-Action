1. 什么是ConfigMap, Secret, 分别有什么作用？
2. 有几种方法可以向部署的app中传递参数？分别是什么？
    * 向容器传递命令行参数
   * 为每个容器设置自定义的环境变量
   * 通过特殊类型的卷将配置文件挂载到容器中。
2.1. 如何在kubernetes中覆盖命令和参数？
   * 在kubernates 中定义容器时，镜像中的ENTRYPOINT和CMD都可以被覆盖。对应的命令时command和args. 通常情况下只需要自定义参数，命令一般很少覆盖
  ```bash
  kind: Pod
  spec:
    containers:
    - image: some/image
      command: ["bin/command"]
      args:["arg1","arg2"]
  ```
2.2. 利用kubectl 创建一个名为hello-config 的ConfigMap，里面有个映射条目为hello=world
  ```
    kubectl create configmap hello-config --from-literal=hello=world
  ```
2.3. `kubectl create -f hello-config.yaml`这个命令可以做什么？
  创建一个configMap
2.4. 创建一个名为test的pod，将hello-config拷贝到pod.conf文件中，将该文件挂在到pod上，并检查是否使用被挂载的文件。
  ```
apiVersion: v1
kind: Pod
metadata:
  name: test
spec:
  containers:
  - image: luksa/fortune:env
    volumeMounts:
    - name: config
      mountPath: pod.conf
      readOnly: true
    ports:
    - containerPort: 80
      protocol: TCP
  volumes:
  - name: config         //卷定义引用hello-config  configMap
    configMap:
      name: hello-config

      //如何检查使用已经挂载： 读取pod.conf文件下的内容
  ```
2.5. 用命令行创建一个名为combine-config的ConfigMap，包含hello=world，以及animal.conf中的条目
  ```
  kubectl create configmap combine-config
   --from-literal=animal.conf
   --from-literal=hello=world
  ```
2.6. 如果在pod中引用不存在的configmap会发生什么？
  答：kubernates会正常调度pod并尝试运行所有的容器，引用不存在的configMap的容器会启动失败，其余容器会正常启动。 如果之后创建了这个缺失的configMap,失败的容器会自动启动。
3. 如何查找pod的敏感数据？
  ``` 
  kubectl get secrets    
  kubectl describe secrets
  ``` 
4. 创建一个自己的generic Secret, 名字为my-secret, 在上次创建的pod上挂在my-secret
```
参见summary中在pod中使用Secret。
```