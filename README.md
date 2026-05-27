<h1 align="center">ML-Agents Points Collection</h1>

<p align="center">
  <a href="#overview">Overview</a> •
  <a href="#screenshots">Screenshots</a> •
  <a href="#requirements">Requirements</a> •
  <a href="#installation">Installation</a> •
  <a href="#license">License</a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/License-MIT-yellow.svg" />
  <img src="https://img.shields.io/badge/Author-SmartMatt-blue" />
</p>

## Overview
The `ml-agents-points-collection` project is a simple game system developed in the Unity engine, enhanced with the dedicated ML-Agents plugin. This project enables the creation of AI agents using reinforcement learning techniques. It features a pre-configured mechanism for agents and a gaming environment where they learn and perform tasks. The agents are placed on platforms where objects containing negative and positive points spawn randomly. The AI's goal is to collect 5 positive points. Collecting a negative point decreases the score by -1, while a positive point increases it by +1. The agent wins by achieving a score of 5 points and loses if its score falls below 0 or if it falls off the platform.

### Agent Mechanics
- Agents are aware of their relative position, their current score, and the position of each point on their platform.
- Information provided to the agents is normalized according to the observation creation conventions outlined in the ML-Agents documentation.
- The system includes a mechanism for manual agent control.

### Training and Models
- The system is capable of training any number of agents simultaneously, where the results and observations of each agent contribute to a common model.
- A pre-trained model is included for testing, and the system also supports training from scratch.

### Playing the Game
The game can be played in two modes:
1. **AI Mode**: The pre-trained or newly trained AI agents will navigate the platforms and try to achieve the goal.
2. **Manual Mode**: Manually control an agent using the built-in control system.

## Screenshots
![Game](https://smartmatt.pl/github/ml-agents-points-collection/training-process.png)
*Learning process for nine agents simultaneously.*

## Requirements
- Unity Version: Unity 6.1
- After installing the appropriate Unity version, a Python 3.10.11 virtual environment must be created using the provided `requirements.txt` file.

## Installation
1. Install Unity version 6.1 from the official Unity website.
2. Clone the `ml-agents-points-collection` repository to your local machine.
3. Set up a Python 3.10.11 virtual environment:
```
python -m venv [env_name]
source [env_name]/bin/activate # On Unix or MacOS
[env_name]\Scripts\activate # On Windows
pip install -r requirements.txt
```
4. Open the project in Unity.
5. Configure the system to your liking according to the guide in the documentation. [[LINK]](https://unity-technologies.github.io/ml-agents/Getting-Started/)

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
&copy; 2025 Mateusz Płonka (SmartMatt). All rights reserved.
<a href="https://smartmatt.pl/">
    <img src="https://smartmatt.pl/github/smartmatt-logo.png" title="SmartMatt Logo" align="right" width="60" />
</a>

<p align="left">
  <a href="https://smartmatt.pl/">Portfolio</a> •
  <a href="https://github.com/SmartMaatt">GitHub</a> •
  <a href="https://www.linkedin.com/in/mateusz-p%C5%82onka-328a48214/">LinkedIn</a> •
  <a href="https://www.youtube.com/user/SmartHDesigner">YouTube</a> •
  <a href="https://www.tiktok.com/@smartmaatt">TikTok</a>
</p>
