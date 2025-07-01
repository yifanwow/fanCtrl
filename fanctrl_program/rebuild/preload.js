const { contextBridge } = require('electron');

// 暂时不暴露任何接口，后面集成 fanctrl 再加
contextBridge.exposeInMainWorld('api', {
  // 后续填入: 调用 fanctrl.exe 等
});
