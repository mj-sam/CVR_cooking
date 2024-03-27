# Collaborative VR Cooking

## License
This project is licensed under the BSD-3-Clause License - see the LICENSE file for details.

## Prerequisites
- Unity version 2021.3.17f1.
- Head-Mounted Display (HMD) or Mock HMD setup for project settings.

## Installation

1. **Clone the Repository**: Utilize Git to clone the repository to your local machine.

    ```
    git clone https://github.com/mj-sam/CVR_cooking
    ```

2. **Open in Unity**: Launch the project in Unity (version 2021.3.17f1). Ensure compatibility and correct version usage.

3. **Download Packages**: Upon opening, Unity will automatically begin downloading necessary packages. Await completion before proceeding.

## Configuration

1. **HMD Settings**: Navigate to `Project Settings` and adjust the HMD settings. For non-HMD environments, select `Mock HMD`.

2. **Server Setup**:
      - open the config file in asset folder : config.xml
      - change the <ServerIp> to the corresponding server IP
      - Note: if you set your IP to the system that plays server and client at the same time please chose host uppon joining the game

2. **Client Setup**: Prepare the environment for server-client interaction.
    - Prepare the Meta quest HMD and Oculus app connection to the PC/Laptop. The game has been tested with AirLink and Meta Cable so chose it based on your need.
    - **Server IP Configuration**: Enter the server IP address in the designated text box within the game or enter it into the network manager entity in unity.
    - **Hosting**: Initiate the server by clicking/selecting the `Host` button.
    - **Joining**: Clients can join the session by clicking/selecting the `Join` button.

## Usage

After setup, the server hosts the environment, allowing clients to connect and interact within the CVR_cooking application. Ensure network configurations are appropriately established for seamless connectivity and interaction.

## Contributing
Contributions are welcome. Please review the contributing guidelines and code of conduct before submitting pull requests or issues.
