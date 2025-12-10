import React from 'react';
import { createRoot } from 'react-dom/client';
import { App } from './init/app.tsx';
import '@coreui/coreui/dist/css/coreui.min.css';
import '@coreui/icons/css/free.min.css';
// bilo gdje u app koristiti predefinisane stilove iz biblioteke UI
import './styles/ui-library.css';
import './styles/coreui-custom.css';

import '@coreui/coreui/dist/css/coreui.min.css';

createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
