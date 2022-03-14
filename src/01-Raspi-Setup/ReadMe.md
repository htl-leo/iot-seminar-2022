# Raspi 4 64bit Setup

## Raspberry Pi Imager

- Download: https://www.raspberrypi.com/software/
- OS: ```Raspberry Pi OS Lite (64 bit)```
- Settings:
    - Overscan deaktivieren
    - Hostname: ```your-name.local```
    - SSH aktivieren (mit Password)
        - Username: ```pi```
        - Passwort: ```iot20220314``` 
    - Wifi einrichten
        - Wifi-Land: AT
    - Spracheinstellungen
        - Zeitzone / Tastaturlayout
        - Einrichtungsassistent *nicht* überspringen
    - Dauerhafte Einstellungen
        - alles aus (kein Auswerfen, keine Telemetrie)

### WLAN Troubleshooting

- Über LAN verbinden
- WLAN händisch konfigurieren (```sudo raspi-config```)
- Oder: ```sudo nano /etc/wpa_supplicant/wpa_supplicant.conf```

    ```
    ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev
    update_config=1
    country=AT

    network={
            ssid="iot"
            psk="iot20220314"
    }
    ```
### System upgraden

- ```sudo apt update && sudo apt full-upgrade```
- ```sudo reboot -h now```

## Samba

### Samba installieren
```sh
sudo apt-get install samba samba-common smbclient
sudo smbpasswd -a pi
```

### Share einrichten
- ```sudo nano /etc/samba/smb.conf```

    ```
    [pi]
        comment = home pi
        path = /home/pi/
        writeable = yes
        browsable = yes
    ```

- ```sudo service smbd restart```
- Testen:
    ```
    sudo service smbd status
    sudo service nmbd status
    ```

### Netzwerklaufwerk verbinden

- Share: ```\\raspi-ip-adresse\pi```
- Verbindung mit anderen Anmeldeinformationen herstellen => pi + Passwort

## Docker

### Mit convenience script installieren

- Scripts Verzeichnis
    ```
    mkdir ~/scripts
    cd ~/scripts
    ```
- Convenience script
    ```
    curl -fsSL https://get.docker.com -o get-docker.sh 
    sudo sh get-docker.sh 
    sudo usermod -aG docker pi
    ```
- Nach Reconnect: ```docker version```

### docker-compose 

- Muß am Raspi über python selbst gebaut werden
    ```
    sudo apt-get install -y libffi-dev libssl-dev
    sudo apt-get install -y python3 python3-pip
    sudo pip3 -v install docker-compose
    ```
- **Kompilieren dauert!**
- ```docker-compose version```

## Git

- ```sudo apt-get install git```
- Seminar-Repo clonen
- Test: Docker-Compose starten
    - In ```src/docker/raspi-arm64``` Verzeichnis wechseln
    - ```docker-compose up -d```   
    - RPI-Monitor: http://raspi-ip:8888


## IoT Docker Services

### Docker Verzeichnisstruktur
- src
    - docker
        - raspi-arm64
            - Ein gemeinsames ```docker-compose.yml``` für alle Services
            - Host bind mounts bei Bedarf (z.B. für Mosquitto)
        - x86-64-host
### Portainer

Übersicht über alle laufenden docker-container 
- Starten über docker-compose: 
    - ```docker-compose up -d portainer```
    - Port ```9000``` (=> http://localhost:9000)
- Beim ersten Laden Benutzer anlegen (```pi``` + pwd)
- Docker-managed volume ```portainer-data```

### LogServer Seq

- Http-Port ```5341```
- Visualisierung (Datensenke) für künftige dotnet log messages
- ```docker-compose up -d seq```

### RPI Monitor

- Http-Port ```8888```
- ```docker-compose up -d rpi-monitor```

### Mosquitto MQTT Broker

- ```docker-compose up -d mqtt```
- Server braucht lokale volume bind mounts für die Konfigurationsdaten (siehe ```config```)
    - Zertifikate für TLS
    - Authorisierung mit user/password

```docker
    volumes:
      - ./mosquitto/config:/mosquitto/config
      - ./mosquitto/log:/mosquitto/log
      - ./mosquitto/data:/mosquitto/data
      - /etc/localtime:/etc/localtime:ro
```

#### Password-File
- mosquitto-Container ohne config starten (oder mit ```allow_anonymous true``` in ```mosquitto.conf```)
- Mit Shell in Container verbinden:
    - ```mosquitto_passwd -c passwordfile user``` (File generieren)
    - ```mosquitto_passwd -b passwordfile user password``` (Neuen User hinzufügen)
    - ```mosquitto_passwd -D passwordfile user``` (User löschen)

#### Mqtt Testen
- MQTT-Client: http://mqtt-explorer.com/
- Connection:
    - Zertifikat nicht validieren
    - Encryption einschalten
    - Host / Port (8883)
    - Username + Password siehe Passwd-File
- Zugriffe im Log-File
    - ```./mosquitto/log```

## Dotnet SDK 6.0 @ Raspi 

### Installation

#### Achtung bei älteren dotnet-Installationen!

- Händisches Entfernen (rm -rf) und Setzen von symlinks auf dotnet-executable notwendig

```sh
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc
```
#### Test mit
```sh
dotnet --info
dotnet --list-sdks
dotnet new console
dotnet build
dotnet run
```

### Erster Test: Simple WebApi

- Neues Projekt mit WebApi Template erzeugen
    - ```dotnet new webapi –n FirstTest```
- cd in Projekt-Folder
    ```
    cd FirstTest
    dotnet build
    ```
- Starten mit Port-Binding
    - ```dotnet run --urls http://0.0.0.0:5000```
- Browser: 
    - http://```pi-ip-adresse```:5000/swagger

### Mit Visual Studio 2022 am Host:
- Solution Name: ```FirstTest```
- Template: ASP.NET Core WebApi
- Enable:
    - HTTPS + Docker Support
    - OpenAPI Support

### WebApi Docker Setup

- Modifikationen im Dockerfile: Build-Step überflüssig
- cd in den sln-Folder
- ```docker build -f ./ApiDemo/Dockerfile -t api-demo .```
- ```docker run -p 8001:80 -d api-demo```
- Browser: http://localhost:8001
    - Weatherforecast: http://localhost:8001/Weatherforecast


#### Swagger-Page
- Swagger (OpenApi-Doku) ist nur im Development Mode integriert!
- Daher in ```Program.cs``` Swagger und SwaggerUI auch für <b>Production Mode</b> aktivieren (in der HTTP Request Pipeline)
- Image neu bauen!
- Browser: http://localhost:8001/swagger

#### Docker-Compose

- cd in den sln-Folder
- ```docker compose up -d```
- Browser: 
    - host: http://localhost:8081/swagger
    - Raspi: http://```pi-ip-adresse```:8081/swagger

#### Known Issue
- Docker build core dump (nur arm32, nicht auf 64bit): https://github.com/dotnet/dotnet-docker/issues/3253




