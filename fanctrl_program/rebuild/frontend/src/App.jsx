import { useState } from 'react';
import ActivityPage from './pages/ActivityPage';
import HomePage from './pages/HomePage';
import TabBar from './components/TabBar';

export default function App() {
  const [currentTab, setCurrentTab] = useState('home');

  return (
    <div className="screen-wrapper">
      <div className="screen-content">
        <TabBar currentTab={currentTab} onTabChange={setCurrentTab} />
        <div className="main-area">
          {currentTab === 'home' ? <HomePage /> : <ActivityPage />}
        </div>
      </div>
    </div>
  );
}
