import React from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './init/app'
import '@coreui/coreui/dist/css/coreui.min.css';
import '@coreui/icons/css/free.min.css';

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)