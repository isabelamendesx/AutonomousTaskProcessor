# Autonomous Task Processor 🤖

 Um processador autônomo de tarefas desenvolvido em C#, elaborado como uma ferramenta para treinamento em programação assíncrona que pode ser empregada em diferentes projetos concretos.

## Descrição

Este projeto é um processador de tarefas que não necessita da intervenção do usuário, desenvolvido para ser uma ferramenta flexível que pode ser integrada em diversos contextos. Desenvolvi este projeto enquanto cursava o módulo Técnicas de Programação I no curso BackEnd Developer C# da Ada Tech, então seu principal
objetivo é demonstrar o uso dos tópicos abordados: Programação assíncrona, Arquivos de Configuração, Collections, LINQ e Interfaces de Coleção.

- `⚙️ Programação Assíncrona`
 Uso extensivo para garantir uma execução responsiva e eficiente das tarefas, permitindo operações I/O paralelas assíncronas, gerenciamento eficaz e espera não bloqueante.
- `🛠️ Arquivos de Configuração`
 Permite configurar informações utilizadas pelo aplicativo de forma fácil como limites de subprocessos por processo, duração mínima e máxima de subprocessos e ainda o número máximo de processos paralelos a serem executados.
- `🗃️ Collections`
 Implementação para armazenamento e manipulação de dados de forma eficiente e segura.
- `🔍 LINQ`
 Aplicado para realizar operações de consulta e manipulação de dados, permitindo a visualização de forma mais eficiente.
- `💉 Injeção de Dependência`
 Padrão empregado através de um contêiner DI para promover desacoplamento e facilitar a gestão de dependências dentro do projeto, centralizando as configurações em um único método na classe Program.cs

## 📁 Estrutura de diretórios
A solução é dividida em três projetos: duas Class Libraries - o Core e a Camada de Acesso a Dados - e um Console App que serve como a Console UI. Futuramente, está planejada a implementação de uma Web API, que compartilhará o mesmo Core. O banco de dados utilizado é o SQLite, com o Entity Framework como ORM.

`AutonomousTaskProcessor (Core)`
- **/Data:** Contém o contexto com o banco de dados e o o arquivo de dados SQLite;
-  **/Entities:** Entidades modelos do projeto;
- **/Migrations:** Migrações geradas pelo EF;
- **/Repositories:** Contém a abstração dos repositórios a serem implementados na camada de dados;
- **/Services:** Diretório de serviços disponíveis na aplicação;

`ConsoleUI`
- **/Resources:** Contém classes com recursos utilizados no projeto de maneira geral;
- **/UI:** Abstrações e classes concretas relacionadas a UI em Console;
- **/Program.cs:** Ponto de entrada da aplicação;

`Repositories`
- **/SqliteRepository** Implementação concreta do repositório definida no Core;
- **/UI:** Abstrações e classes concretas relacionadas a UI em Console;

# 🔧 Como executar?

## Pré-requisitos
Certifique-se de ter o Visual Studio instalado em seu sistema antes de prosseguir.

### Passo 1: Obtenha o código-fonte
Clone o repositório do projeto em seu ambiente local ou faça o download dos arquivos fonte em um diretório de sua escolha.

### Passo 2: Abra o projeto no Visual Studio
Abra o Visual Studio e carregue a solução com os projetos (.sln) no ambiente de desenvolvimento.

### Passo 3: Compile e Execute o aplicativo
No Visual Studio, clique no botão "Build" para compilar o projeto. Certifique-se de que não há erros durante o processo de compilação.

Após a compilação bem-sucedida, clique no botão "Start" (ou pressione F5) para iniciar a execução do aplicativo.

# 👥 Autores

| [<img src="https://avatars.githubusercontent.com/u/129897959?v=4" width=115><br><sub>Isabela Gomes</sub>](https://github.com/isabelamendesx)  |
| :---: | 
