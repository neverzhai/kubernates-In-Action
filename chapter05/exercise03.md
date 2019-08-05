# 5.6

## What is a headless service

## What does headless service used for
用于发现独立的pod 
## Define a headless service with YAML file
```yaml
spec:
  ClusterIP: None

```
## How to discover pods that behind a headless service through DNS
在集群中运行的一个pod中执行DNS查询，借助Docker Hub上的一个tutum/dnsutils镜像来完成。P157

## How to define a servie that will discover all pods-even those that a not ready
P158

