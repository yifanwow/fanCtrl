
import HardwareInfoBlock from '../components/HardwareInfoBlock';
import FanSpeedBlock from '../components/FanSpeedBlock';
import PumpSpeedBlock from '../components/PumpSpeedBlock';

export default function HomePage() {
  return (
    <div>
      <HardwareInfoBlock />
      <FanSpeedBlock />
      <PumpSpeedBlock />
    </div>
  );
}
