import { useState } from 'react';
import ActivityPage from './pages/ActivityPage';
import HomePage from './pages/HomePage';
import TabBar from './components/TabBar';

export default function App() {
  const [currentTab, setCurrentTab] = useState('home');

  return (
    <div>
      <TabBar currentTab={currentTab} onTabChange={setCurrentTab} />
      <div style={{ padding: '20px' }}>
        {currentTab === 'home' ? <HomePage /> : <ActivityPage />}
      </div>
    </div>
  );
}
