export default function TabBar({ currentTab, onTabChange }) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', padding: '12px', borderBottom: '1px solid #444' }}>
        <button
          className={currentTab === 'activity' ? 'active-tab' : ''}
          onClick={() => onTabChange('activity')}
        >
          活动
        </button>
        <button
          className={currentTab === 'home' ? 'active-tab' : ''}
          onClick={() => onTabChange('home')}
        >
          主页
        </button>
      </div>
    );
  }
  