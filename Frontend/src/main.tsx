import React from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './init/app.tsx';
// bilo gdje u app koristiti predefinisane stilove iz biblioteke UI
import './styles/ui-library.css';

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
