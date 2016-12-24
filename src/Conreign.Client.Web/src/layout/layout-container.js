import React from 'react';

export function LayoutContainer({ children }) {
  return (
    <div>
      <header>
        Header 23
      </header>
      <main>
        {children}
      </main>
      <footer>
        Footer
      </footer>
    </div>
  );
}

export default LayoutContainer;
