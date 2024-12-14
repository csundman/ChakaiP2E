"use client";

import { useState } from "react";
import Nav from "./Nav";
import { Breadcrumb } from "./components/Breadcrumb";
import { CharacterProvider } from "./contexts/CharacterContext";
import LoginPage from "./components/LoginPage";
import { useAuth } from '../lib/AuthContext';

// Create a new component to use session
function AppShellContent({ children }: { children: React.ReactNode }) {
  const [isNavOpen, setIsNavOpen] = useState(false);
  const { isAuthenticated } = useAuth();

  const toggleNav = () => {
    setIsNavOpen(!isNavOpen);
  };

  if (!isAuthenticated) {
    return <LoginPage />;
  }

  return (
    <CharacterProvider>
      <div className="flex h-full flex-col">
        <button
          className="md:hidden p-4 hover:bg-gray-100"
          onClick={toggleNav}
          aria-label={isNavOpen ? "Close menu" : "Open menu"}
        >
          {isNavOpen ? (
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          ) : (
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          )}
        </button>
        <div className="flex flex-1 flex-col md:flex-row overflow-hidden">
          <Nav className={`w-full h-auto md:w-64 md:h-full border-b md:border-b-0 md:border-r ${isNavOpen ? '' : 'hidden md:block'}`} />
          <main className="flex-1 overflow-auto">
            <div className="px-4">
              <Breadcrumb />
            </div>
            <div className="p-4">
              {children}
            </div>
          </main>
        </div>
        <footer className="border-t p-4 text-center text-sm bg-gray-950">
          Created by Kairi Sundman and Chad Sundman 2024
        </footer>
      </div>
    </CharacterProvider>
  );
}

// Main AppShell component that provides the session
export default function AppShell({ children }: { children: React.ReactNode }) {
  return <AppShellContent>{children}</AppShellContent>;
} 