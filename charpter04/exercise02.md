## Part 1 Replication set
* Replace replication controller by replica set
    * 基于第五次作业，请将replication controller替换成replica set.要求：
        * 替换后，replica set所管理的pod不允许做任何改动
        * 替换后，pod的replica count仍然是3
        * 替换过程中，不允许手动重建pod
        * 替换后，原有的replication controller不允许仍然存在于集群中

* Replica set's expression label selectors  
We have six pods as following  
[  
    {
     PodName: AppOne,
     labels:
        {   FrontEnd: React,
            Language: JS,
            Webpack: v4.0.0,
            Handsontable: v7.0.0
        }
    },  
    {
     PodName: AppTwo,
     labels:
        {   FrontEnd: Vue,
            Language: TypeScript,
            Webpack: v4.0.0,
        }
    },  
    {
     PodName: AppThree,
     labels:
        {   FrontEnd: Angular,
            Language: PHP,
            Webpack: v4.0.0,
        }
    },  
    {
     PodName: AppFour,
     labels:
        {   FrontEnd: React,
            Language: Js,
        }
    },  
    {
     PodName: AppFive,
     labels:
        {   FrontEnd: React,
            Language: js,
            Webpack: v4.0.1,
        }
    },  
    {
     PodName: AppSix,
     labels:
        {   FrontEnd: Angular,
            Language: TypeScript,
            Webpack: v5.0.1,
        }
    },
]  

It also can be described by a table:  

| PodName | label: FrontEnd | label: Language | label: Webpack | label: Handsontable |
| --- | --- | --- | --- | --- |
| AppOne | React | JS | V4.0.0 | v7.0.0 |
| AppTwo | Vue | TypeScript | V4.0.0 | / |
| AppThree | Angular | PHP | V4.0.0 | / |
| AppFour | React | js | / | / |
| AppFive | React | Js | V4.0.1 | / |
| AppSix | Angular | TypeScript | V5.0.3 | / |

Please create a ReplicaSet which only manage AppFive and AppSix.

## Part 2 DaemonSet
* please create a daemonset make all nodes which has the label "hello:world" has a pod you created on practice 5.

## Part 3 Job
* 创建一个docker image,当container启动后会输出$“hello world {currentTime}”,例如：
> hello world 2019/6/8 22:12:22  // 时间格式不限，请精确到秒。
* 创建一个Job，让其连续运行以上镜像5次。
* 创建一个Job，让其并发运行以上镜像3次。
* 创建一个cron job, 让其每隔30秒运行一次