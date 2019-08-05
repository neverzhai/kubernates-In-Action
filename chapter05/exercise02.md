# 5.5
## What is `readiness probes`
定期调用，并确定pod是否可以接收客户端请求
## When should we use `readiness probes`
启动时最有用

## How many types do `readiness probes` have and what are they
3
- Exec探针
- HTTP GET探针
- TCP socket探针

## What operations does `readiness probe` do
 就绪探针会定期调用，并确定特定的pod是否接收客户端请求，但容器准备就绪，探测返回成功时，表示容器已经准备好接收请求。

## What is the important distiction between `liveness probes` and `readiness probes`
如果容器未通过准备检查，则不会倍终止或重新启动，存活探针时通过杀死异常的容器并用新的正常的容器代替它们来保证pod正常工作，就绪探针是确保只有准备好处理请求的pod才可以接收它们请求。
## Why `readiness probe` is important
P152 就绪探针确保客户端只与正常的pod交互，客户端永远不会知道系统存在问题。
## Define a readiness probe in the .yaml file of a ReplicaSet

- it should be Exec probe
- it should be first invoke 5 seconds after the pod starts
- it should be check every 10 seconds
- it should be defined with 2 seconds timeout

```yaml
readinessProbe:
        exec:
          command:
          - ls
          - /var/ready
        initialDelaySeconds: 5
        periodSeconds: 10
        timeout: 2  
```
## In the real world, how do we manully add/ remove a pod from a service
P154, 通过删除pod或者手动更改标签。
## Why should we always define a readiness prob
P155.
## Why there need not to include pod shutdown logic into your readiness probe
P155， 啥意思？？
