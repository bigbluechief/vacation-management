module.exports = {
  lintOnSave: false,
  outputDir: '../wwwroot/app',
  publicPath: '/app/',
  devServer: {
    proxy: 'https://localhost:5001'
  }
}