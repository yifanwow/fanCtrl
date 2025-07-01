const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow() {
  const win = new BrowserWindow({
    width: 500,
    height: 790,
    resizable: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    }
  });

  // ✅ 保持宽高比为 9:14（大约就是 450:700）
  win.setAspectRatio(9 / 14);

  // 加载打包后的前端文件
//   win.loadFile(path.join(__dirname, 'frontend/dist/index.html'));
win.loadURL("http://localhost:5173");
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
