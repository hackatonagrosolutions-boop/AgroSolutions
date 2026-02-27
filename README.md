# 🌾 AgroSolutions - Hackathon Fase 5

> **Projeto de Conclusão de Pós-Graduação**
> Solução robusta de Agricultura 4.0 focada em monitoramento IoT, escalabilidade e observabilidade.

---

## 📺 Apresentação e Documentação
Para uma imersão completa na solução, utilize os links abaixo:

* 🎥 **Vídeo de Apresentação:** [Assistir no Google Drive](https://drive.google.com/file/d/1h7RSpzyaMNpsUk0Dxfv6LLGn_p36B85H/view?usp=sharing)
* 📐 **Diagrama de Arquitetura:** [Explorar no Miro](https://miro.com/app/board/uXjVG85GX0s=/?share_link_id=873000372835)

---

## 🛠️ Tecnologias e Ferramentas

### **Backend**
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

### **Orquestração**
![Kubernetes](https://img.shields.io/badge/kubernetes-%23326ce5.svg?style=for-the-badge&logo=kubernetes&logoColor=white)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)

### **Mensageria**
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)
### **Bancos de Dados**
![MicrosoftSQLServer](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![MongoDB](https://img.shields.io/badge/MongoDB-%234ea94b.svg?style=for-the-badge&logo=mongodb&logoColor=white)

### **Observabilidade**
![Prometheus](https://img.shields.io/badge/Prometheus-E6522C?style=for-the-badge&logo=Prometheus&logoColor=white)
![Grafana](https://img.shields.io/badge/Grafana-F46800?style=for-the-badge&logo=Grafana&logoColor=white)

### **CI/CD**
![GitHub Actions](https://img.shields.io/badge/github%20actions-%232671E5.svg?style=for-the-badge&logo=githubactions&logoColor=white)

---

## 📖 Estudo de Caso
A **AgroSolutions** é uma cooperativa agrícola tradicional que busca se modernizar para enfrentar os desafios do século XXI: otimização de recursos hídricos, aumento da produtividade e sustentabilidade.

Atualmente, a tomada de decisão no campo é baseada majoritariamente na experiência dos agricultores. Com a visão de implementar a **Agricultura 4.0**, esta plataforma de IoT (Internet of Things) e análise de dados oferece aos seus cooperados um serviço de precisão baseado em telemetria em tempo real.

---

## 🏗️ Arquitetura da Solução
A aplicação foi estruturada seguindo os princípios de microsserviços e orquestrada via **Kubernetes**, fundamentada em quatro pilares técnicos:

### 1. Persistência Poliglota (Polyglot Persistence)
* **SQL Server:** Utilizado pelos serviços `AuthService`, `PropriedadeService` e `AlertaService`, garantindo transações ACID para dados relacionais críticos.
* **MongoDB:** Implementado no `SensorService` para suportar a alta vazão e a natureza semiestruturada (JSON) dos dados de telemetria.

### 2. Comunicação Assíncrona e Desacoplamento
O **RabbitMQ** atua como Message Broker central. Isso permite que o `AlertaService` consuma eventos de forma reativa, evitando que picos de tráfego nos sensores afetem a disponibilidade dos demais serviços.

### 3. Ingestão Inteligente
O `SensorService` funciona como um **API Gateway especializado**, realizando o parsing e a validação dos dados antes da persistência e publicação na fila, protegendo a integridade do ecossistema.

---

## 🖥️ Swagger

### **AuthService**
![](AUTHSERVICE.png)

### **PropriedadeService**
![](PROPRIEDADESERVICE.png)

### **SensorService**
![](SENSORSERVICE.png)

### **AlertaService**
![](ALERTASERVICE.png)

---

## 🧪 Teste Unitário
![](TESTEUNITARIO.png)

---

## 📨 Mensageria
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white) 
![](RABBITMQ.png)

---

## 🗄️ Banco de Dados
## ![MicrosoftSQLServer](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)  **AuthService** - **PropriedadeService** - **AlertaService**
![](SQLSERVER.png)

## ![MongoDB](https://img.shields.io/badge/MongoDB-%234ea94b.svg?style=for-the-badge&logo=mongodb&logoColor=white) **SensorService**
![](MONGODB.png)

---

## 📊 Infraestrutura e Observabilidade
Um diferencial crítico deste projeto é a mentalidade **DevOps** aplicada desde a concepção:

* **Monitoramento:** Pipeline de métricas com **Prometheus**, realizando o *scraping* automático de endpoints do Kubernetes.
* **Visualização:** Dashboards em **Grafana** para análise de *throughput*, latência e saúde operacional dos pods.
* **Deployment:** Esteira automatizada via **GitHub Actions**, garantindo a integridade do código através de pipelines de CI/CD.

---

## 🧩 Microsserviços
| Serviço | Responsabilidade | Banco de Dados |
| :--- | :--- | :--- |
| **AuthService** | Cadastro e autenticação do Produtor Rural | SQL Server |
| **PropriedadeService** | Gestão de Propriedades e Talhões | SQL Server |
| **SensorService** | Ingestão e parsing de telemetria IoT | MongoDB |
| **AlertaService** | Gravação de histórico e processamento de alertas | SQL Server |

---

## 🚀 Requisitos Técnicos Atendidos
- [x] Arquitetura baseada em Microsserviços.
- [x] Orquestração via Kubernetes (K8s).
- [x] Observabilidade completa (Prometheus/Grafana).
- [x] Desacoplamento via Mensageria (RabbitMQ).
- [x] Pipeline de CI/CD automatizado via GitHub Actions.

---

## 🛳️ Orquestração Local com Docker Compose

O arquivo `docker-compose-ghcr.yml` é responsável por orquestrar todos os microsserviços, bancos de dados, mensageria e ferramentas de observabilidade da solução AgroSolutions em um ambiente local, utilizando contêineres Docker. Ele facilita o deploy e a execução de todos os componentes necessários para o funcionamento completo do sistema, sem a necessidade de instalar cada dependência manualmente.

> **Nota:** Se você preferir buildar os serviços localmente (em vez de usar as imagens do registry), utilize o arquivo `docker-compose.yml`. Ele irá construir as imagens a partir do código fonte presente na sua máquina.

### Serviços Orquestrados
- **authservice**: Serviço de autenticação e cadastro de usuários, utiliza SQL Server como banco de dados.
- **propriedadeservice**: Gerencia propriedades e talhões, também utiliza SQL Server.
- **sersorservice**: Responsável pela ingestão e parsing de dados de sensores IoT, utiliza MongoDB.
- **alertaservice**: Processa e armazena alertas, utiliza SQL Server.
- **sqlserver**: Banco de dados relacional utilizado pelos serviços Auth, Propriedade e Alerta.
- **mongodb**: Banco de dados NoSQL utilizado pelo SensorService.
- **rabbitmq**: Broker de mensageria para comunicação assíncrona entre microsserviços.
- **prometheus**: Coleta métricas dos serviços para monitoramento.
- **grafana**: Visualização de métricas e dashboards customizados.

Cada serviço é iniciado em seu próprio contêiner, com as variáveis de ambiente e dependências já configuradas para integração automática.

### Como Executar Localmente

#### Pré-requisitos
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/) instalados na máquina.

#### Passos para execução
1. Abra um terminal na raiz do projeto (onde está o arquivo `docker-compose-ghcr.yml`).
2. Execute os comandos:
   
   ```sh
   docker login ghcr.io -u hackatonagrosolutions-boop -p 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
   docker compose -f docker-compose-ghcr.yml up --pull always --build
   ```
   
   Isso irá baixar as imagens necessárias do GitHub Container Registry (GHCR), criar os contêineres e iniciar todos os serviços.

3. Acompanhe os logs no terminal para verificar o status dos serviços.
4. Após a inicialização, os serviços estarão disponíveis nas seguintes portas:
   - AuthService: http://localhost:5001/swagger
   - PropriedadeService: http://localhost:5002/swagger
   - SensorService: http://localhost:5003/swagger
   - AlertaService: http://localhost:5004/swagger
   - SQL Server: localhost:1433
   - MongoDB: localhost:27017
   - RabbitMQ: http://localhost:15672 (usuário/padrão: guest/guest)
   - Prometheus: http://localhost:9090
   - Grafana: http://localhost:3000 (senha padrão: admin)

5. Para parar e remover os contêineres, utilize:
   
   ```sh
   docker compose -f docker-compose-ghcr.yml down
   ```

> **Dica:** O arquivo já está configurado para garantir que as dependências (bancos, mensageria) estejam disponíveis antes de subir os microsserviços, facilitando o uso local para desenvolvimento, testes e demonstrações.

---

## 🏁 Conclusão
A arquitetura proposta demonstra maturidade ao separar preocupações de negócio de preocupações de infraestrutura. O uso de **Namespaces** no Kubernetes e **NodePorts** específicos garante uma organização lógica e acesso controlado, resultando em um sistema resiliente, fácil de monitorar e pronto para o crescimento sob demanda no agronegócio moderno.

---
