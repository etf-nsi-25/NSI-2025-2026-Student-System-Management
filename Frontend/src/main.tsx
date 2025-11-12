import React from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './init/app.tsx';

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
