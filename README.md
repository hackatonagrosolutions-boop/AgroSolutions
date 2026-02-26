# 🌾 AgroSolutions - Hackathon Fase 5

> **Projeto de Conclusão de Pós-Graduação**
> Solução robusta de Agricultura 4.0 focada em monitoramento IoT, escalabilidade e observabilidade.

---

## 📺 Apresentação e Documentação
Para uma imersão completa na solução, utilize os links abaixo:

* 🎥 **Vídeo de Apresentação:** [Assistir no Google Drive](https://drive.google.com/file/d/1obK1rZlVQMg1Ae3IBzCLjT1LOipDIqRj/view?usp=sharing)
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
![](TESTE UNITARIO.png)

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

## 🏁 Conclusão
A arquitetura proposta demonstra maturidade ao separar preocupações de negócio de preocupações de infraestrutura. O uso de **Namespaces** no Kubernetes e **NodePorts** específicos garante uma organização lógica e acesso controlado, resultando em um sistema resiliente, fácil de monitorar e pronto para o crescimento sob demanda no agronegócio moderno.
