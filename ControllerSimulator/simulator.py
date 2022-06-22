from nis import match
import random
import time
import json


from paho.mqtt import client as mqtt_client

broker = '10.135.16.167'
port = 1883
client_id = f'simulator-{random.randint(0, 1000)}'
username = 'guest'
password = 'guest'

def connect_mqtt():
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
        else:
            print("Failed to connect, return code %d\n", rc)
    client = mqtt_client.Client(client_id)
    client.username_pw_set(username, password)
    client.on_connect = on_connect
    client.connect(broker, port)
    return client

def publish(client):
    temp_last_val = float(random.randint(12, 25))
    temp_count = 0
    hum_last_val = float(random.randint(20, 50))
    hum_count = 0

    while True:
        time.sleep(1)

        add = go_up_down_or_stand(hum_count, hum_last_val)

        if add == -1:
            hum_last_val -= .25
            hum_count = 0
        if add == 1:
            hum_last_val += .25
            hum_count = 0
        
        _publish(client, "home/humidity", "humidity", hum_last_val)
        hum_count += 1

        time.sleep(1)

        add = go_up_down_or_stand(temp_count, temp_last_val)

        if add == -1:
            temp_last_val -= .25
            temp_count = 0
        if add == 1:
            temp_last_val += .25
            temp_count = 0

        _publish(client, "home/temperature", "temperature", temp_last_val)
        temp_count += 1

def _publish(client, topic, type, val):
    msg = json.dumps({ type: val })
    result = client.publish(topic, msg)
    # result: [0, 1]
    status = result[0]
    if status == 0:
        print(f"Send `{msg}` to topic `{topic}`")
    else:
        print(f"Failed to send message to topic {topic}")

def go_up_down_or_stand(count, value):
    rand = abs(random.randint(1, 100) - 100)
    if rand <= count*.33:
        if rand<count*.33/2:
            return -1
        else:
            return +1
    else:
        return 0
        

def run():
    client = connect_mqtt()
    client.loop_start()
    publish(client)

if __name__ == '__main__':
    run()