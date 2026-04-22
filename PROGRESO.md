# SHATTER — Registro de Progreso

## Sprint 0 — Reestructuración a Platformer 2D con fondos 2.5D

### Fecha: 2026-04-10

---

## Decisión de reestructuración

El proyecto arrancó como 2.5D **isométrico** con movimiento diagonal. Se decide descartar todo lo hecho y reestructurar como **platformer 2D estilo Mario** con fondos 2.5D (parallax multicapa + profundidad visual), manteniendo únicamente el **concepto base** de SHATTER (Mateo, salud mental adolescente, Destellos de Lucidez). La narrativa, niveles y mecánicas originales quedan como referencia en `doc.md` pero se reinventan.

**Decisiones clave:**
- Perspectiva: platformer 2D (A/D camina, Espacio/W salta, S agacha), fondos 2.5D parallax
- Movimiento: habilidades avanzadas (salto variable, coyote time, jump buffer, doble salto, wall slide + wall jump, dash, drop-through)
- Combate: no convencional — esquivar + power-ups + debuffs + personalización
- Alcance del sprint: core + Nivel 1 prototipo jugable
- **Borrado:** carpeta `_SHATTER` anterior, sprites de Mateo isométricos, tilesets Classroom/Industrial/DevilsWork, FogOfWar, PROGRESO.md antiguo

**Requisitos UPN preservados:**
- Singleton: `GameManager`, `ObjectPool`, `AudioManager`
- Object Pooling: `ObjectPool` + `IPoolable`, usado por coleccionables/VFX
- Personalización del PJ: Fragmentos de Identidad (`IdentityFragmentSO` + `IdentityManager`)
- ScriptableObjects: `IdentityFragmentSO`, `PowerUpSO`, `DebuffSO`

---

## Estructura creada

```
Assets/_SHATTER/
├── Scripts/
│   ├── Core/               GameManager, ObjectPool, AudioManager, GameEvents
│   ├── Player/             PlayerController2D, PlayerHealth, PlayerAnimatorBridge,
│   │                       InteractionSystem, IdentityManager
│   ├── Systems/            PowerUpManager, DebuffManager, Collectible, Checkpoint
│   ├── Camera/             CameraFollow2D, ParallaxLayer
│   ├── AI/                 EnemyBase, WalkerEnemy, FlyerEnemy
│   ├── Levels/             LevelManager, Level01Manager, LevelGoal,
│   │                       OneWayPlatform, Hazard
│   ├── UI/                 HUDManager, PauseMenu
│   ├── Utils/              PlaceholderSpriteGenerator2D, PlaceholderTileGenerator2D,
│   │                       SceneBootstrapper
│   └── ScriptableObjects/  IdentityFragmentSO, PowerUpSO, DebuffSO
├── ScriptableObjects/      (Identities/, PowerUps/ — vacíos, se crean en runtime)
├── Sprites/                (Player/, Enemies/, Tiles/, Props/, Parallax/ — vacíos)
├── Prefabs/                (vacíos)
├── Audio/, Animations/, Materials/  (vacíos)
```

---

## Scripts implementados

### Core (Shatter.Core)

**[Assets/_SHATTER/Scripts/Core/GameManager.cs](Assets/_SHATTER/Scripts/Core/GameManager.cs)**
- Singleton DontDestroyOnLoad. Tracking de Destellos, nivel actual, checkpoint, estado (Playing/Paused/GameOver/Cutscene), muertes.
- Eventos: `OnDestellosChanged`, `OnPlayerDeath`, `OnLevelCompleted`, `OnStateChanged`.
- API: `AddDestellos`, `SetCheckpoint`, `GetRespawnPosition`, `TogglePause`, `LoadLevel`, `RestartCurrentScene`.

**[Assets/_SHATTER/Scripts/Core/ObjectPool.cs](Assets/_SHATTER/Scripts/Core/ObjectPool.cs)**
- Pool genérico tag-based. `Spawn(tag, pos, rot)` / `Despawn(tag, obj)`. Interfaz `IPoolable { OnSpawn, OnDespawn }`. Registrable desde Inspector o via `RegisterPool(tag, prefab, initialSize)`.

**[Assets/_SHATTER/Scripts/Core/AudioManager.cs](Assets/_SHATTER/Scripts/Core/AudioManager.cs)**
- 3 AudioSources (music/sfx/ambient). `PlayMusic/PlaySFX/PlayAmbient`, volúmenes master/music/sfx/ambient.

**[Assets/_SHATTER/Scripts/Core/GameEvents.cs](Assets/_SHATTER/Scripts/Core/GameEvents.cs)**
- Bus de eventos estáticos. Player, Collectibles, Enemigos, Power-ups, Debuffs, Cámara.

### Player (Shatter.Player)

**[Assets/_SHATTER/Scripts/Player/PlayerController2D.cs](Assets/_SHATTER/Scripts/Player/PlayerController2D.cs)**
- Platformer 2D avanzado: aceleración/fricción, salto variable, coyote time (0.12s), jump buffer (0.15s), **doble salto**, **wall slide + wall jump**, **dash** (intangible), **crouch**, **drop-through** desde OneWayPlatforms.
- Ground check y wall check con `Physics2D.OverlapBox`.
- Input legacy: A/D horizontal, Espacio/W salto, LeftShift/K dash, S crouch/drop.
- API: `StopMovement`, `ResumeMovement`, `ApplySpeedMultiplier`, `SetControlsInverted`, `SetExtraJumps`.

**[Assets/_SHATTER/Scripts/Player/PlayerHealth.cs](Assets/_SHATTER/Scripts/Player/PlayerHealth.cs)**
- Vida por "Fragmentos" (4 default). IFrames 1s con blink, knockback, respawn por checkpoint. `TakeDamage(int, sourcePos)`, `SetDamageReduction`.

**[Assets/_SHATTER/Scripts/Player/PlayerAnimatorBridge.cs](Assets/_SHATTER/Scripts/Player/PlayerAnimatorBridge.cs)**
- Puente opcional con Animator (speed, vy, grounded, dashing, wallSliding). Voltea SpriteRenderer por facing.

**[Assets/_SHATTER/Scripts/Player/InteractionSystem.cs](Assets/_SHATTER/Scripts/Player/InteractionSystem.cs)**
- `Physics2D.OverlapCircleNonAlloc` detecta `IInteractable` más cercano. Tecla E ejecuta.

**[Assets/_SHATTER/Scripts/Player/IdentityManager.cs](Assets/_SHATTER/Scripts/Player/IdentityManager.cs)**
- Máximo 3 Fragmentos de Identidad equipados. Recalcula tint del sprite, speed multiplier, extra jumps, damage reduction, particle effects al equipar/desequipar.

### Systems (Shatter.Systems)

**[Assets/_SHATTER/Scripts/Systems/PowerUpManager.cs](Assets/_SHATTER/Scripts/Systems/PowerUpManager.cs)**
- Solo 1 power-up activo. Tipos: *Latido Calmado* (slowdown enemigos en radio), *Destello Guía* (resalta coleccionables), *Escudo Respiración* (ignora 1 golpe), *Memoria Clara* (+velocidad), *Foco Claridad* (+salto extra).

**[Assets/_SHATTER/Scripts/Systems/DebuffManager.cs](Assets/_SHATTER/Scripts/Systems/DebuffManager.cs)**
- Solo 1 debuff activo. *Tunnel Vision* (lento), *Pasos de Plomo* (salto reducido), *Controles Invertidos*. El escudo del PowerUp consume automáticamente un debuff entrante.

**[Assets/_SHATTER/Scripts/Systems/Collectible.cs](Assets/_SHATTER/Scripts/Systems/Collectible.cs)**
- `CollectibleKind.Destello/PowerUp/IdentityFragment`. Bob sinusoidal, highlight pulse, devuelve al pool al recoger si hay pool registrado.

**[Assets/_SHATTER/Scripts/Systems/Checkpoint.cs](Assets/_SHATTER/Scripts/Systems/Checkpoint.cs)**
- Trigger que registra posición en `GameManager.SetCheckpoint`. Cambia color al activarse.

### Camera (Shatter.CameraSystem)

**[Assets/_SHATTER/Scripts/Camera/CameraFollow2D.cs](Assets/_SHATTER/Scripts/Camera/CameraFollow2D.cs)**
- `Vector3.SmoothDamp` + look-ahead según velocidad X del target. Bounds opcionales. Screen shake via `GameEvents.OnCameraShake`.

**[Assets/_SHATTER/Scripts/Camera/ParallaxLayer.cs](Assets/_SHATTER/Scripts/Camera/ParallaxLayer.cs)**
- Mueve el transform proporcional al delta de cámara. `parallaxFactorX/Y` 0..1.5. `infiniteScrollX` opcional.

### AI (Shatter.AI)

**[Assets/_SHATTER/Scripts/AI/EnemyBase.cs](Assets/_SHATTER/Scripts/AI/EnemyBase.cs)**
- Abstracta. Daño al contacto + debuff al player. Jump-stomp: si el player cae encima con velocidad negativa, el enemigo recibe daño y el player rebota.

**[Assets/_SHATTER/Scripts/AI/WalkerEnemy.cs](Assets/_SHATTER/Scripts/AI/WalkerEnemy.cs)**
- Patrulla horizontal, gira al detectar borde o pared con raycast. Aplica *Tunnel Vision*.

**[Assets/_SHATTER/Scripts/AI/FlyerEnemy.cs](Assets/_SHATTER/Scripts/AI/FlyerEnemy.cs)**
- Patrulla entre 2 puntos ignorando gravedad con bob sinusoidal. Aplica *Controles Invertidos*.

### Levels (Shatter.Levels)

**LevelManager** (abstracta), **Level01Manager**, **LevelGoal**, **OneWayPlatform**, **Hazard**.

### UI (Shatter.UI)

**[Assets/_SHATTER/Scripts/UI/HUDManager.cs](Assets/_SHATTER/Scripts/UI/HUDManager.cs)**
- HUD construido en código: contador de destellos, vida, power-up activo con tiempo, debuff activo con tiempo, prompt de interacción.

**[Assets/_SHATTER/Scripts/UI/PauseMenu.cs](Assets/_SHATTER/Scripts/UI/PauseMenu.cs)**
- ESC pausa/despausa. Panel con Reanudar / Inventario Emocional / Reiniciar Nivel / Salir. Inventario permite equipar/desequipar Fragmentos de Identidad.

### Utils (Shatter.Utils)

**PlaceholderSpriteGenerator2D** — genera rects, círculos y gradientes en runtime.
**PlaceholderTileGenerator2D** — construye el Nivel 1 procedural con piso, huecos, escalones, plataforma flotante, pared para wall-jump, zona baja, one-way platforms.
**SceneBootstrapper** — ensambla TODO al dar Play.

### ScriptableObjects

`IdentityFragmentSO`, `PowerUpSO`, `DebuffSO` — data containers con menú `Create → Shatter → ...`.

---

## ⚠️ Configuración previa en Unity

**Antes de dar Play**, debes crear estas Layers en `Edit → Project Settings → Tags and Layers`:

| Índice sugerido | Layer Name |
|---|---|
| 6 | Ground |
| 7 | Wall |
| 8 | OneWay |
| 9 | Enemy |
| 10 | Interactable |

Y crear el tag `Player` (si no existe) y aplicárselo al objeto Player.

Si no creas las layers, el prototipo igual corre pero el ground check, wall check y one-way detection serán imprecisos (todo queda en `Default`).

---

## Cómo probar

1. Abrir Unity, cargar `Assets/Scenes/SampleScene.unity`.
2. Vaciar la escena (si tiene objetos de ensayos previos).
3. Crear un GameObject vacío llamado **"Bootstrapper"**.
4. Adjuntarle el script `Shatter.Utils.SceneBootstrapper`.
5. Dejar todos los flags activos en el Inspector.
6. Dar **Play**.

### Controles

| Input | Acción |
|---|---|
| A / ← | Caminar izquierda |
| D / → | Caminar derecha |
| Espacio / W / ↑ | Saltar (hold = salto más alto) |
| Espacio en aire | Doble salto |
| Espacio contra pared | Wall jump |
| LeftShift / K | Dash horizontal |
| S / ↓ | Agacharse |
| S + Espacio sobre one-way | Drop-through |
| E | Interactuar |
| ESC | Pausa / Inventario Emocional |

### Checklist de verificación

- [ ] Player aparece a la izquierda del mapa y cae al piso.
- [ ] Camina izq/der con A/D.
- [ ] Salta, doble salto funciona en aire.
- [ ] Wall slide al pegarse a la pared cerca del final del mapa.
- [ ] Wall jump hacia el lado opuesto.
- [ ] Dash con Shift.
- [ ] Agacharse con S.
- [ ] Drop-through en las 3 plataformas amarillas (one-way).
- [ ] Recoger destellos amarillos — contador HUD sube.
- [ ] Tocar el Walker rojo → daño + debuff *Tunnel Vision* (viñeta/lento) 4s.
- [ ] Tocar el Flyer morado → debuff *Controles Invertidos* 3s.
- [ ] Recoger power-up cyan → HUD muestra "Latido Calmado" con timer.
- [ ] Recoger power-up verde → escudo persistente.
- [ ] Tocar el pit rojo → muerte instantánea, respawn en checkpoint.
- [ ] Tocar checkpoint gris → cambia a amarillo, posición de respawn se actualiza.
- [ ] ESC abre pausa, tiempo se detiene.
- [ ] Botón "Inventario Emocional" → lista 3 Fragmentos de Identidad (Aura de la Púa / Tinta de Estrellas / Acero Mental).
- [ ] Click en fragmento → se equipa, tint del player cambia.
- [ ] Parallax: mover la cámara horizontalmente muestra capas moviéndose a distintas velocidades.

---

## Pendiente / fuera de alcance

- Sprites reales, animaciones, tilesets artísticos.
- Niveles 2-5 y nivel secreto.
- Sistemas narrativos (secuencia del teléfono, clasificación de recuerdos, anclas, confrontación con La Sombra).
- Audio real (música, SFX, VO).
- Menú principal, save/load persistente, settings.
- Build para Android/WebGL.
- Reemplazar input legacy por el nuevo Input System (ya hay `InputSystem_Actions.inputactions` en el proyecto).

---

## Sprint 1 — Sistemas v2.1 (planeado, no implementado)

### Fecha de planeación: 2026-04-11

[doc.md](doc.md) fue refinado a v2.1 con nuevos sistemas que requieren implementación. El doc es el GDD vigente; este sprint los baja a código.

### Scripts a crear

| Script | Namespace | Responsabilidad |
|---|---|---|
| **`FocusSystem.cs`** | `Shatter.Player` | Stamina con chispas (3 default), consumo en dash/doble salto/wall jump (1 cada uno), recarga grounded 1.2s, recarga total al tocar Luz Cálida o Santuario, modificadores por nivel via `IFocusModifier` |
| **`EmotionalWeight.cs`** | `Shatter.Player` | Peso emocional global (0-100%). Sube por daño/quietud/preguntas evasivas. Baja por Anclas/Bancas/Memorias positivas/Santuarios. Aplica efectos por umbral (recarga focus −25%, max focus −1, salto −25%). Visual de niebla en sprite. |
| **`SanctuarySystem.cs`** | `Shatter.Systems` | Save real persistente (escribe a disco), heal completo, depósito de Destellos, reequipar fragmentos, mostrar mapa del nivel, comerciar con Eco Amable. Activación E sostenido 2s con animación ritual. |
| **`WarmLight.cs`** | `Shatter.Systems` | Objeto ambiental. Al tocar: recarga focus 100% + heal +1 + −20% peso. Único uso. Reset al Desvanecerse. Compatible con Light2D. |
| **`TeaItem.cs`** | `Shatter.Systems` | Consumible de inventario. Stack máx 3. Tecla H. Heal +1. Animación 1.5s. |
| **`SanctuaryDataSO.cs`** | `Shatter.ScriptableObjects` | Data container: nombre temático, sprite, motif corto, descripción ambiental por nivel |
| **`ConsumableSO.cs`** | `Shatter.ScriptableObjects` | Data container: efecto, animación, stack máximo, ícono |

### Scripts a modificar

| Script | Cambio |
|---|---|
| **`PlayerHealth.cs`** | Formalizar fragmentos como Aliento. Implementar `IHealable`. Añadir cap dinámico por Galletas/Raíz Cálida/Perdón. Distinción daño estándar (-1) vs hazard duro (Desvanecerse). |
| **`PlayerController2D.cs`** | Consumir 1 chispa de `FocusSystem` en cada `OnDoubleJump`, `OnDash`, `OnWallJump`. Bloquear acciones si no hay chispas. Stomp llama `FocusSystem.AddSpark(1)`. |
| **`PowerUpManager.cs`** | Añadir power-up **Suspiro Profundo** que llama `PlayerHealth.Heal(2)`. Marcar MVP/F2 según doc §11. |
| **`Checkpoint.cs` → `Bench.cs`** | Renombrar. Comportamiento sin cambio (respawn point). Añadir entrega de Té de Tilo en primera activación. NO save persistente — eso es Santuario. |
| **`HUDManager.cs`** | Refactor a HUD diegético: solo Destellos seguros + bolsa + prompt + Tés. Quitar barra de HP, barra de stamina, barra de power-up timer. Mover Aliento (halo), Enfoque (chispas orbitando) y Peso (niebla) al sprite del player. |
| **`GameManager.cs`** | Añadir save persistente real (JSON a disco). Sistema de bolsa vs seguros para Destellos. Tracking de muertes consecutivas en misma room para mercy system (Luz Cálida temporal tras 3 muertes). Implementar fail state "Desvanecerse" con secuencia de §13.c. |
| **`Hazard.cs`** | Distinción `HazardType` (Soft = -1 fragmento, Hard = Desvanecerse instantáneo). Tinte rojo automático para Hard. |

### Reglas de testing del Sprint 1

1. Crear Layer adicional `Sanctuary` (índice 11). El Santuario aplica enemigos no entran en su radio.
2. La tecla **H** debe registrarse en `InputSystem_Actions.inputactions` para uso de Té de Tilo.
3. Validar que el sistema de bolsa/seguros sobreviva al Desvanecerse.
4. Validar que las 5 frases rotativas de Desvanecerse aparecen aleatorias.
5. Validar que al respawnear tras Desvanecerse, las Luces Cálidas que se habían apagado vuelven a encenderse (reset por muerte).
6. Validar mercy system: 3 muertes consecutivas en misma room → Luz Cálida temporal aparece.

### No incluido en Sprint 1

- Mapa visual del nivel en Santuario (UI compleja — fase 2).
- Comercio con Eco Amable (UI de tienda — fase 2).
- Lectura de diarios desde Santuario (UI de libros — fase 2).
- Save automático en cuartos del nivel secreto (el nivel secreto sigue siendo fase 2).
