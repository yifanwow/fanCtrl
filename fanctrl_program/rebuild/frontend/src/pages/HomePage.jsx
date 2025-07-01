
import HardwareInfoBlock from '../components/HardwareInfoBlock';
import FanSpeedBlock from '../components/FanSpeedBlock';
import PumpSpeedBlock from '../components/PumpSpeedBlock';

export default function HomePage() {
    return (
      <div style={{ display: 'flex', flexDirection: 'column', flex: 1 }}>
        <HardwareInfoBlock />
        <FanSpeedBlock />
        <PumpSpeedBlock />
      </div>
    );
  }
  
