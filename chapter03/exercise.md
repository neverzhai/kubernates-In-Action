# leaning-k8s-in-action

## Part1 pod

请简述：

1. 什么是node?
2. 什么是pod?
3. pod和node是什么关系？
4. pod和docker container是什么关系？

    基于第三次作业请回答以下问题:

    ```yml
    apiVersion: apps/v1beta1
    kind: Deployment
    metadata:
      name: myapp-webapi
    spec:
      replicas: 1
      strategy:
        rollingUpdate:
          maxSurge: 1
          maxUnavailable: 1
      minReadySeconds: 5
      template:
        metadata:
          labels:
            app: myapp-webapi
        spec:
          containers:
          - name: myapp-webapi
            image: fqdong.azurecr.io/myapp_webapi:latest
            ports:
            - containerPort: 80
            resources:
              requests:
                cpu: 250m
              limits:
                cpu: 500m
    apiVersion: v1
    kind: Service
    metadata:
      name: myapp-webapi
    spec:
      type: LoadBalancer
      ports:
      - port: 8181
        targetPort: 80
      selector:
        app: myapp-webapi
    ```

* 当前集群有几个node?
    ```bash
    kubectl get nodes
    ```
* 当前集群有几个pod？
    `kubectl get pods`
* 怎么查询asp.net core app所在的pod运行在哪个node上？

## Part2 labels

请简述：

* 什么是labels？
* labels有什么用
请分别通过yaml文件和command，给任意pod加上以下标签：hello-label=world
请分别通过yaml文件和command，将上述标签修改为：hi-label=universe
请使用command，查询：
* 含有标签"hi-label"的所有pod
* 不含有标签"hi-label"的所有pod
* 含有标签"hi-lable"且值为"universe"的所有pod
* 含有标签"hi-lable"且值不为"universe"的所有pod
请使用command删除含有标签"hi-label"的所有pod

## part3 namespace

请简述：

* 什么是namespace？
* namespace有什么用

* 请分别通过yaml文件和command，创建一个名为world的namespace

  ```yaml
  # filename: world.yaml
  apiVersion: v1
  kind: Namespace
  metadata:
    name: world
  ```

  ```bash
  kubectl create -f world.yaml
  ```

  ```bash
  kubectl create namespace world
  ```

* 请分别通过yaml文件和command，创建一个pod使其隶属于上述创建的namespace

  ```yaml
  # filename: kubia-manual.yaml
  apiVersion: v1
  kind: Pod
  metada:
    name: kubia-manual
    namespace: world
  spec:
    containers:
      - image: luksa/kubia
        name: kubia
        ports:
          - containerPort: 8080
            protocol: TCP
  ```

  ```bash
  kubectl create -f kubia-manual.yaml -n world
  ```

改

* 请分别通过yaml文件和command，将上述namespace的名字修改为universe

  * yaml way

    ```yaml
    # filename: kubia-manual.yaml
    apiVersion: v1
    kind: Namespace
    metadata:
      name: universe
    ---
    apiVersion: v1
    kind: Pod
    metada:
      name: kubia-manual
      namespace: universe
    spec:
      containers:
        - image: luksa/kubia
          name: kubia
          ports:
            - containerPort: 8080
              protocol: TCP
    ```

    ```bash
    kubectl create -f  kubia-manual.yaml
    kubectl delete ns world
    ```

  * command way

    ```bash
    kubectl create ns universe
    ```

    ```yaml
    # filename: kubia-manual.yaml
    apiVersion: v1
    kind: Pod
    metada:
      name: kubia-manual
      namespace: universe
    spec:
      containers:
        - image: luksa/kubia
          name: kubia
          ports:
            - containerPort: 8080
              protocol: TCP
    ```

    ```bash
    kubectl create -f kubia-manual.yaml -n universe
    kubectl delete ns world
    ```
* 请使用command，查询所有namespace

  ```bash
  kubectl get ns
  ```
* 请使用command，查询universe中的所有pod
  ```bash
  kubectl get po --namespace universe
  ```
* 请使用command，删除universe中的所有pod，但保留该namespace

  ```bash
  kubectl delete po --all --namespace universe
  ```
