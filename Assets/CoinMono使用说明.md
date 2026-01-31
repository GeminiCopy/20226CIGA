# CoinMono使用说明

## 🎯 功能概述
CoinMono脚本实现了一个金币拾取系统，当Player碰撞金币时会自动销毁金币并添加新摄像机。

## 🔧 实现功能
- ✅ **Player碰撞检测**：检测Player layer的碰撞对象
- ✅ **自我销毁**：拾取后自动销毁金币对象
- ✅ **摄像机添加**：调用CameraManager添加新子摄像机
- ✅ **调试输出**：控制台输出拾取信息

## 📋 组件配置要求

### Coin对象设置：
1. **添加BoxCollider2D组件**
   - 设置 `IsTrigger = true`
   - 调整Collider大小匹配金币

2. **添加CoinMono脚本**
   - 自动处理碰撞逻辑

### Player对象设置：
1. **确保Layer设置为"Player"**
2. **添加Rigidbody2D组件**
   - Body Type: Dynamic
   - 确保可以触发碰撞

## 🎮 预期行为

### 碰撞流程：
1. Player与Coin碰撞
2. OnTriggerEnter2D触发
3. 检测Player layer
4. 添加新子摄像机到CameraManager
5. 销毁Coin对象

### 控制台输出：
```
Coin被玩家拾取，添加新摄像机
CameraManager: Added sub camera X (from prefab/dynamic)
```

## ⚠️ 注意事项

1. **场景依赖**：确保场景中存在CameraManager实例
2. **Layer配置**：Player对象必须使用"Player" layer
3. **组件完整性**：Coin需要Collider2D，Player需要Rigidbody2D
4. **触发器设置**：Coin的Collider必须启用IsTrigger

## 🧪 测试验证

1. **播放场景**：Player移动碰撞Coin
2. **验证输出**：检查控制台日志
3. **确认效果**：观察Coin消失和新摄像机添加