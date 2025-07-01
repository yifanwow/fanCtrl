export default function PumpSpeedBlock() {
    return (
        <div style={{
            flex: 1,
            padding: '16px',
            boxSizing: 'border-box',
            borderBottom: '1px solid #222'
          }}>
        <h3>水泵转速</h3>
        <p>每次有新信号时刷新。</p>
      </div>
    );
  }
  