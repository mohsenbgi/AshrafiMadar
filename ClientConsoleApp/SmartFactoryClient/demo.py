#!/usr/bin/env python3
"""
🏭 Smart Factory Client Demo Script
Demonstrates how the client application works with simulated Proteus data
"""

import time
import random
import subprocess
import threading
import sys
import signal

class ProteusSensorSimulator:
    """Simulates Proteus sending sensor data via virtual COM port"""
    
    def __init__(self):
        self.running = False
        self.base_values = {
            'Furnace_Temp': 850.0,
            'Env_Humid': 50.0,
            'Light_Level': 200,
            'Gas_Methane': 20,
            'Gas_CO': 50,
            'Machine_Sound': 500,
            'Tank_Pressure': 70,
            'Main_Current': 120,
            'Engine_Vibe': 20,
            'Input_Voltage': 230,
            'Conveyor_Dist': 150,
            'Water_Leak': 100,
            'Coolant_Valve': 85
        }
        
    def generate_sensor_data(self):
        """Generate realistic sensor data with random variations"""
        # Add random variations to base values
        furnace_temp = self.base_values['Furnace_Temp'] + random.uniform(-50, 100)
        env_humid = max(0, min(100, self.base_values['Env_Humid'] + random.uniform(-15, 15)))
        light_level = max(0, self.base_values['Light_Level'] + random.randint(-100, 150))
        gas_methane = max(0, self.base_values['Gas_Methane'] + random.randint(-10, 50))
        gas_co = max(0, self.base_values['Gas_CO'] + random.randint(-20, 60))
        machine_sound = max(0, self.base_values['Machine_Sound'] + random.randint(-200, 300))
        tank_pressure = max(0, min(100, self.base_values['Tank_Pressure'] + random.randint(-20, 30)))
        main_current = max(0, self.base_values['Main_Current'] + random.randint(-40, 80))
        engine_vibe = max(0, self.base_values['Engine_Vibe'] + random.randint(-10, 40))
        input_voltage = max(0, self.base_values['Input_Voltage'] + random.randint(-50, 50))
        conveyor_dist = max(0, self.base_values['Conveyor_Dist'] + random.randint(-100, 100))
        water_leak = max(0, self.base_values['Water_Leak'] + random.randint(-50, 200))
        coolant_valve = max(0, min(100, self.base_values['Coolant_Valve'] + random.randint(-20, 30)))
        
        # Random critical events (low probability)
        flame_status = 1 if random.random() < 0.03 else 0  # 3% chance
        gate_status = 1 if random.random() < 0.15 else 0   # 15% chance
        e_stop_button = 1 if random.random() < 0.01 else 0 # 1% chance
        
        # Create the data string in the expected format
        header = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE"
        data = (f"Furnace_Temp:{furnace_temp:.2f},Env_Humid:{env_humid:.2f},"
                f"Light_Level:{light_level},Gas_Methane:{gas_methane},Gas_CO:{gas_co},"
                f"Machine_Sound:{machine_sound},Tank_Pressure:{tank_pressure},"
                f"Main_Current:{main_current},Engine_Vibe:{engine_vibe},"
                f"Input_Voltage:{input_voltage},Conveyor_Dist:{conveyor_dist},"
                f"Water_Leak:{water_leak},Flame_Status:{flame_status},"
                f"Gate_Status:{gate_status},E_Stop_Button:{e_stop_button},"
                f"Coolant_Valve:{coolant_valve}")
        
        return f"{header}\n{data}"
    
    def get_status_emoji(self, data):
        """Get status emojis based on data values"""
        emojis = []
        
        if "Flame_Status:1" in data:
            emojis.append("🔥")
        if "Gate_Status:1" in data:
            emojis.append("🚪")
        if "E_Stop_Button:1" in data:
            emojis.append("🛑")
        if "Gas_Methane:" in data:
            methane = int(data.split("Gas_Methane:")[1].split(",")[0])
            if methane > 300:
                emojis.append("⚠️")
        if "Tank_Pressure:" in data:
            pressure = int(data.split("Tank_Pressure:")[1].split(",")[0])
            if pressure > 90:
                emojis.append("💥")
                
        return " ".join(emojis) if emojis else "✅"

def print_banner():
    """Print demo banner"""
    print("""
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║     🏭 Smart Factory Client - DEMO MODE                     ║
║                                                               ║
║     📡 Simulated Proteus Data Generator                      ║
║     🔗 Virtual COM Port Communication                        ║
║     🌐 API Integration Testing                               ║
║                                                               ║
║     This demo shows how the client receives data from        ║
║     Proteus and forwards it to the Smart Factory API         ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
""")

def print_instructions():
    """Print usage instructions"""
    print("""
🚀 Demo Instructions:
===================

1. This script simulates Proteus sending sensor data
2. The data will be sent directly to the Smart Factory API
3. You can view the results in the web dashboard at http://localhost:5000
4. The demo generates realistic sensor data with random variations
5. Occasionally, critical events (fire, emergency stop) will be triggered

📊 Data Format:
===============
Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:XXX.XX,Env_Humid:XX.XX,Light_Level:XXX,Gas_Methane:XX,Gas_CO:XX,
Machine_Sound:XXX,Tank_Pressure:XX,Main_Current:XXX,Engine_Vibe:XX,
Input_Voltage:XXX,Conveyor_Dist:XXX,Water_Leak:XXX,Flame_Status:X,
Gate_Status:X,E_Stop_Button:X,Coolant_Valve:XX

🎮 Controls:
============
Press Ctrl+C to stop the demo at any time
""")

def run_demo():
    """Run the demo simulation"""
    simulator = ProteusSensorSimulator()
    
    print_banner()
    print_instructions()
    
    print("🔍 Testing API connectivity...")
    
    # Test API connectivity
    import requests
    try:
        response = requests.get("http://localhost:5000/api/sensor/current", timeout=5)
        if response.status_code == 200:
            print("✅ API server is running and accessible")
        else:
            print(f"⚠️  API server responded with status: {response.status_code}")
    except requests.exceptions.RequestException:
        print("❌ API server is not accessible. Make sure it's running at http://localhost:5000")
        print("   Start the main application first: dotnet run (in the main project folder)")
        return
    
    print("\n🎭 Starting sensor data simulation...")
    print("📡 Sending data to Smart Factory API every 3 seconds")
    print("🌐 View real-time updates at: http://localhost:5000")
    print("\nPress Ctrl+C to stop\n")
    
    data_count = 0
    
    try:
        while True:
            data_count += 1
            
            # Generate sensor data
            sensor_data = simulator.generate_sensor_data()
            status_emojis = simulator.get_status_emoji(sensor_data)
            
            # Send to API
            try:
                response = requests.post(
                    "http://localhost:5000/api/sensor/raw",
                    data=sensor_data,
                    headers={"Content-Type": "text/plain"},
                    timeout=10
                )
                
                if response.status_code == 200:
                    # Extract key values for display
                    lines = sensor_data.split('\n')
                    data_line = lines[1]
                    
                    # Parse key values
                    temp = data_line.split("Furnace_Temp:")[1].split(",")[0]
                    pressure = data_line.split("Tank_Pressure:")[1].split(",")[0]
                    gas_co = data_line.split("Gas_CO:")[1].split(",")[0]
                    
                    print(f"📡 [{data_count:03d}] Data sent {status_emojis} | "
                          f"Temp: {float(temp):.1f}°C | "
                          f"Pressure: {pressure} PSI | "
                          f"CO: {gas_co} ppm | "
                          f"Status: ✅ Sent")
                else:
                    print(f"❌ [{data_count:03d}] API Error: {response.status_code}")
                    
            except requests.exceptions.RequestException as e:
                print(f"❌ [{data_count:03d}] Connection Error: {str(e)[:50]}...")
            
            # Wait before next transmission
            time.sleep(3)
            
    except KeyboardInterrupt:
        print(f"\n\n🛑 Demo stopped by user")
        print(f"📊 Total data packets sent: {data_count}")
        print("👋 Thank you for trying the Smart Factory Client demo!")

if __name__ == "__main__":
    try:
        run_demo()
    except Exception as e:
        print(f"\n❌ Demo error: {e}")
        sys.exit(1)