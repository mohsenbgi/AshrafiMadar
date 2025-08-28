#!/usr/bin/env python3
"""
🏭 Smart Factory Monitoring System - API Test Script
Test script for sending sensor data to the monitoring system
"""

import requests
import json
import time
import random

API_BASE_URL = "http://localhost:5000/api"

def test_normal_data():
    """Test sending normal sensor data"""
    print("📡 Sending normal sensor data...")
    
    data = ("Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n"
           "Furnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61,"
           "Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230,"
           "Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90")
    
    try:
        response = requests.post(f"{API_BASE_URL}/sensor/raw", 
                               data=data, 
                               headers={"Content-Type": "text/plain"})
        print(f"✅ Status: {response.status_code}")
        print(f"📊 Response: {response.json()}")
    except Exception as e:
        print(f"❌ Error: {e}")

def test_warning_data():
    """Test sending warning sensor data"""
    print("\n⚠️ Sending warning sensor data...")
    
    data = ("Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n"
           "Furnace_Temp:950.00,Env_Humid:85.00,Light_Level:50,Gas_Methane:250,Gas_CO:85,"
           "Machine_Sound:650,Tank_Pressure:85,Main_Current:160,Engine_Vibe:60,Input_Voltage:200,"
           "Conveyor_Dist:0,Water_Leak:450,Flame_Status:0,Gate_Status:1,E_Stop_Button:0,Coolant_Valve:95")
    
    try:
        response = requests.post(f"{API_BASE_URL}/sensor/raw", 
                               data=data, 
                               headers={"Content-Type": "text/plain"})
        print(f"✅ Status: {response.status_code}")
        print(f"📊 Response: {response.json()}")
    except Exception as e:
        print(f"❌ Error: {e}")

def test_emergency_data():
    """Test sending emergency sensor data"""
    print("\n🚨 Sending emergency sensor data...")
    
    data = ("Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n"
           "Furnace_Temp:1050.00,Env_Humid:90.00,Light_Level:30,Gas_Methane:400,Gas_CO:120,"
           "Machine_Sound:700,Tank_Pressure:95,Main_Current:180,Engine_Vibe:80,Input_Voltage:180,"
           "Conveyor_Dist:0,Water_Leak:500,Flame_Status:1,Gate_Status:1,E_Stop_Button:1,Coolant_Valve:100")
    
    try:
        response = requests.post(f"{API_BASE_URL}/sensor/raw", 
                               data=data, 
                               headers={"Content-Type": "text/plain"})
        print(f"✅ Status: {response.status_code}")
        print(f"📊 Response: {response.json()}")
    except Exception as e:
        print(f"❌ Error: {e}")

def get_current_data():
    """Get current sensor data"""
    print("\n📊 Getting current sensor data...")
    
    try:
        response = requests.get(f"{API_BASE_URL}/sensor/current")
        print(f"✅ Status: {response.status_code}")
        data = response.json()
        if data['success']:
            sensor_data = data['data']
            print(f"🌡️ Furnace Temperature: {sensor_data.get('furnace_Temp', 'N/A')}°C")
            print(f"💧 Humidity: {sensor_data.get('env_Humid', 'N/A')}%")
            print(f"💡 Light Level: {sensor_data.get('light_Level', 'N/A')} lux")
            print(f"⚠️ Active Warnings: {len(sensor_data.get('activeWarnings', []))}")
            print(f"🚨 Active Alarms: {len(sensor_data.get('activeAlarms', []))}")
    except Exception as e:
        print(f"❌ Error: {e}")

def simulate_continuous_data():
    """Simulate continuous sensor data with random variations"""
    print("\n🔄 Starting continuous data simulation...")
    print("Press Ctrl+C to stop")
    
    base_values = {
        'furnace_temp': 850.0,
        'humidity': 50.0,
        'light_level': 200,
        'gas_methane': 20,
        'gas_co': 50,
        'machine_sound': 500,
        'tank_pressure': 70,
        'main_current': 120,
        'engine_vibe': 20,
        'input_voltage': 230,
        'conveyor_dist': 150,
        'water_leak': 100,
        'coolant_valve': 85
    }
    
    try:
        while True:
            # Add random variations
            furnace_temp = base_values['furnace_temp'] + random.uniform(-50, 50)
            humidity = max(0, min(100, base_values['humidity'] + random.uniform(-10, 10)))
            light_level = max(0, base_values['light_level'] + random.randint(-50, 50))
            gas_methane = max(0, base_values['gas_methane'] + random.randint(-5, 15))
            gas_co = max(0, base_values['gas_co'] + random.randint(-10, 20))
            machine_sound = max(0, base_values['machine_sound'] + random.randint(-100, 200))
            tank_pressure = max(0, min(100, base_values['tank_pressure'] + random.randint(-10, 15)))
            main_current = max(0, base_values['main_current'] + random.randint(-20, 40))
            engine_vibe = max(0, base_values['engine_vibe'] + random.randint(-5, 15))
            input_voltage = max(0, base_values['input_voltage'] + random.randint(-20, 20))
            conveyor_dist = max(0, base_values['conveyor_dist'] + random.randint(-50, 50))
            water_leak = max(0, base_values['water_leak'] + random.randint(-20, 100))
            coolant_valve = max(0, min(100, base_values['coolant_valve'] + random.randint(-10, 15)))
            
            # Random events
            flame_status = 1 if random.random() < 0.02 else 0  # 2% chance of fire
            gate_status = 1 if random.random() < 0.1 else 0    # 10% chance of open gate
            e_stop = 1 if random.random() < 0.005 else 0       # 0.5% chance of emergency stop
            
            data = (f"Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n"
                   f"Furnace_Temp:{furnace_temp:.2f},Env_Humid:{humidity:.2f},Light_Level:{light_level},"
                   f"Gas_Methane:{gas_methane},Gas_CO:{gas_co},Machine_Sound:{machine_sound},"
                   f"Tank_Pressure:{tank_pressure},Main_Current:{main_current},Engine_Vibe:{engine_vibe},"
                   f"Input_Voltage:{input_voltage},Conveyor_Dist:{conveyor_dist},Water_Leak:{water_leak},"
                   f"Flame_Status:{flame_status},Gate_Status:{gate_status},E_Stop_Button:{e_stop},"
                   f"Coolant_Valve:{coolant_valve}")
            
            response = requests.post(f"{API_BASE_URL}/sensor/raw", 
                                   data=data, 
                                   headers={"Content-Type": "text/plain"})
            
            print(f"📡 Data sent - Status: {response.status_code} | "
                  f"Temp: {furnace_temp:.1f}°C | "
                  f"Pressure: {tank_pressure} PSI | "
                  f"Warnings: {'🔥' if flame_status else ''}{'🚪' if gate_status else ''}{'🛑' if e_stop else ''}")
            
            time.sleep(5)  # Send data every 5 seconds
            
    except KeyboardInterrupt:
        print("\n⏹️ Simulation stopped")

def main():
    """Main test function"""
    print("🏭 Smart Factory Monitoring System - API Test")
    print("=" * 50)
    
    # Test basic functionality
    test_normal_data()
    time.sleep(2)
    
    test_warning_data()
    time.sleep(2)
    
    test_emergency_data()
    time.sleep(2)
    
    get_current_data()
    
    print("\n" + "=" * 50)
    print("🌐 Dashboard available at: http://localhost:5000")
    print("📡 API endpoints available:")
    print("  - POST /api/sensor/raw")
    print("  - POST /api/sensor/data")
    print("  - GET /api/sensor/current")
    print("  - GET /api/sensor/locations")
    print("  - POST /api/test/simulate")
    print("  - POST /api/test/simulate-warning")
    print("  - POST /api/test/simulate-emergency")
    
    # Ask if user wants continuous simulation
    choice = input("\n🔄 Do you want to start continuous data simulation? (y/N): ")
    if choice.lower() == 'y':
        simulate_continuous_data()

if __name__ == "__main__":
    main()