# SHATTER — Documento de Diseño

> **Versión 2.1 — Refinamiento de sistemas (2026-04-11)**
>
> Esta es la versión vigente del documento. La v2.0 (platformer 2D inicial, 2026-04-10) sigue intacta en estructura — la v2.1 **añade** los sistemas que faltaban (Aliento, Enfoque, Save de dos tiers, fail state "Desvanecerse", Destellos como moneda, Luces Cálidas, ramas de exploración reales) y reconcilia las contradicciones internas. Los niveles se mantienen pero ahora declaran explícitamente qué sistemas usan y cuántos puntos seguros tienen. La versión 2.5D isométrica queda deprecada desde 2026-04-10. El registro técnico está en [PROGRESO.md](PROGRESO.md).

---

# I. INTRODUCCIÓN Y CONCEPTO

## 1. Información general

| Campo | Detalle |
|---|---|
| **Nombre** | SHATTER |
| **Género** | Platformer 2D narrativo / exploración emocional |
| **Tema** | Salud mental adolescente |
| **Perspectiva** | Platformer lateral 2D con fondos 2.5D (parallax multicapa + profundidad) |
| **Referencias visuales** | *Ori and the Blind Forest*, *Rayman Legends*, *Celeste*, *Hollow Knight*, *Gris* |
| **Motor** | Unity (2D + URP) |
| **Plataformas** | PC, WebGL (Android en fases posteriores) |
| **Duración estimada** | 90-120 minutos (5 niveles + nivel secreto) |
| **Curso** | Desarrollo de Videojuegos — Ciclo 9 |
| **Universidad** | UPN |
| **Responsabilidad social** | Concientización sobre salud mental adolescente |

**Sobre el formato 2.5D lateral:** El jugador controla a Mateo en una vista lateral clásica de platformer (A/D camina, Espacio salta). La sensación "2.5D" viene de los **fondos renderizados en múltiples capas con parallax** que dan profundidad visual — siluetas de edificios lejanos, niebla volumétrica, elementos arquitectónicos que pasan por delante del jugador, iluminación dinámica. El gameplay es puramente 2D, pero el mundo *se siente* tridimensional.

## 2. Premisa

> *"A veces la mente más ruidosa es la que más necesita ser escuchada."*

SHATTER es la historia de **Mateo**, un adolescente de 17 años que un día despierta y descubre que no puede salir de su propia mente. El mundo exterior sigue girando, pero él está atrapado adentro — en un universo interior hecho de miedos, recuerdos y pequeñas luces que todavía no se apagan. Para volver a casa debe atravesar cinco mundos que representan estados emocionales, enfrentar a una sombra que es él mismo, y aprender que **pedir ayuda no es rendirse**.

## 3. Pilares de diseño

1. **Movimiento expresivo.** Mateo tiene habilidades completas de platformer (doble salto, wall-jump, dash). El movimiento no es solo locomoción — es un lenguaje emocional. En ansiedad es frenético, en depresión es pesado, en recuperación fluye.
2. **Combate no convencional con costo de movimiento.** No hay armas ni HP de enemigos en el sentido clásico. Los enemigos son **manifestaciones emocionales** que aplican **debuffs** al contacto. Se esquivan, se confrontan con power-ups, o se "derrotan" narrativamente. Y el moverte cuesta — cada dash, cada doble salto y cada wall jump consume **Enfoque** (ver §9.c). Eso también es la metáfora: cuando estás ansioso, te falta el aire para hacer las cosas que sí podrías hacer en calma.
3. **Personalización significativa.** Los **Fragmentos de Identidad** cambian cómo Mateo se ve, se mueve y resiste. Cada equipamiento es una elección sobre qué parte de sí mismo trae al frente.
4. **Fondos que cuentan historia.** Las capas de parallax no son decoración — muestran lo que Mateo ve cuando nadie lo ve.
5. **El jugador empatiza, no resuelve.** No hay un *final bueno* comprable. Hay *presencia*.

## 4. Arco emocional del jugador

| Momento | Nivel | Emoción buscada |
|---|---|---|
| Apertura | 1 | "Algo no está bien" — reconocimiento, inquietud |
| Exploración | 2 | Nostalgia, confusión, ternura incómoda |
| Fondo | 3 | Empatía profunda, vacío, paciencia |
| Giro | 4 | Confrontación, miedo, aceptación |
| Cierre | 5 | Alivio, calidez, dignidad |
| Epílogo (secreto) | ? | "No estoy solo" |

---

# II. HISTORIA Y PERSONAJES

## 5. Protagonista — Mateo

- **Edad:** 17 años
- **Personalidad:** Curioso, callado, con mucho mundo adentro
- **Hobbies (antes):** Tocar guitarra solo en su cuarto, armar maquetas de ciudades imaginarias
- **Conflicto:** Su mejor amigo **Bruno** se alejó sin explicación. Mateo dejó de tocar la guitarra y dejó de hablar de cómo se siente.
- **Frase:** *"Es como si todo fuera demasiado alto y yo no alcanzara a llegar a nada."*
- **Apariencia:** Sprite 2D lateral con cuatro animaciones base (idle / run / jump / fall) y variaciones emocionales que se activan según los Fragmentos equipados. Cuando está agobiado, su silueta "tiembla" con micro-vibraciones; cuando está entero, deja un rastro suave de luz al correr.
- **Paleta:** Fría (azules, grises, tonos urbanos) que se calienta a naranjas/dorados conforme avanza.

## 6. Antagonista — LA SOMBRA

La Sombra **no es un villano**. Es la parte de Mateo que aprendió a quedarse quieta para no sentir. Aparece como una silueta idéntica a la suya pero negra, que en ocasiones lo imita en planos paralelos de fondo (parallax layer "near background") — caminando en dirección opuesta, parada en una cornisa observándolo, a veces *adelante* del jugador esperándolo.

- **Comportamiento:** No ataca directamente. Altera el mundo, susurra frases en texto flotante, distorsiona las capas de fondo cuando Mateo se acerca.
- **Frases:** *"Si no intentas nada, nadie puede decepcionarte."* / *"Todos ya se fueron. ¿Para qué llamarlos?"*
- **Confrontación (Nivel 4):** Primera vez que ocupa el mismo plano de gameplay que Mateo. No es un boss fight de daño — es una secuencia de esquivas de Fragmentos Rotos mientras eliges opciones de diálogo.
- **Resolución:** Mateo le extiende la mano. La Sombra se vuelve pequeña y camina a su lado como compañera en el Nivel 5 (cambia la animación de idle: ahora hay dos siluetas).

## 7. Enemigos — Manifestaciones

Cada enemigo representa una emoción negativa concreta y aplica un **debuff** al contacto en vez de daño numérico puro.

### Los Ecos — Nivel 1 (Ansiedad)
- **Aspecto:** Siluetas humanoides traslúcidas que repiten frases en loop.
- **Comportamiento:** Patrullan plataformas horizontalmente. Al detectar al jugador, lo persiguen acelerando.
- **Debuff:** **Tunnel Vision** — viñeta oscura cierra la pantalla, velocidad reducida 40% durante 4 segundos.
- **Contramedida:** Power-up *Latido Calmado* los ralentiza en un radio.

### Las Voces — Nivel 2 (Confusión / Memoria)
- **Aspecto:** Bocas flotantes sin cuerpo, rodeadas de niebla distorsionada.
- **Comportamiento:** Estáticas. Crean **zonas de confusión** visibles (área semitransparente) al acercarte.
- **Debuff:** **Controles Invertidos** — A↔D durante 3 segundos al entrar en su zona.
- **Contramedida:** Power-up *Silenciador* las neutraliza por 12s.

### Los Pesos — Nivel 3 (Depresión)
- **Aspecto:** Esferas oscuras semitransparentes que flotan lentamente descendiendo.
- **Comportamiento:** Se adhieren a Mateo si pasa cerca. Cada Peso adherido reduce la altura de salto un 10%.
- **Debuff:** **Pasos de Plomo** — salto reducido al 50% durante 5 segundos al adherirse.
- **Contramedida:** Usar una **Ancla Sensorial** cercana disuelve todos los Pesos adheridos.

### Fragmentos Rotos — Nivel 4 (Confrontación)
- **Aspecto:** Pedazos de espejo animados que orbitan en patrones circulares.
- **Comportamiento:** Orbitan alrededor de espejos gigantes. Bloquean pasillos y rutas de salto.
- **Debuff:** **Flash de Recuerdo** — la pantalla se pone en blanco 1 segundo (vulnerabilidad visual, no daño).
- **Contramedida:** Elegir la opción de diálogo correcta con La Sombra los desactiva en un radio.

### La Raíz — Nivel 5 (final, opcional)
- **Aspecto:** Zarcillos vegetales oscuros que brotan del suelo cuando Mateo se queda quieto demasiado tiempo.
- **Comportamiento:** Pasivo. Solo aparece si el jugador no avanza en 15 segundos.
- **Debuff:** Ralentiza la cámara y añade un susurro de "no te muevas". No daña.
- **Contramedida:** Avanzar.

## 8. NPCs de apoyo

### La Chispa (todos los niveles)
Pequeña luz cálida que flota en la capa de gameplay. Si el jugador está perdido 15 segundos, se mueve lentamente en la dirección correcta. No habla — emite un tono armónico.

### El Eco Amable (Niveles 2-5)
Silueta traslúcida con brillo cálido (inverso visual de Los Ecos enemigos). Es la voz interior positiva de Mateo. Aparece en zonas seguras (cornisas iluminadas, bancos, fogatas). Al interactuar con E, ofrece frases de aliento y entrega un power-up o un Fragmento de Identidad.

- **Nivel 2:** *"No todo lo que guardas te pesa. Algunas cosas te sostienen."*
- **Nivel 3:** *"No tienes que nadar. Solo flotar ya es avanzar."*
- **Nivel 4:** *"El espejo no miente. Pero tampoco cuenta toda la historia."*
- **Nivel 5:** *"Ya casi. Llegar tarde no es lo mismo que no llegar."*

### Las Memorias Vivas (Nivel 5)
Versiones pasadas de Mateo a distintas edades (7, 10, 12, 14, 16). Cada una está en una sub-zona del Nivel 5 y entrega un **Fragmento de Identidad** tras una mini-interacción.

### Bruno (Nivel secreto)
El mejor amigo. Solo aparece si el jugador encuentra la **Llave Oxidada** del Nivel 3. Sentado en una banca al final de un nivel lineal, solo. Mateo se sienta a su lado. No hay combate. Solo escuchar. Desbloquea un epílogo.

---

# III. JUGABILIDAD Y SISTEMAS

## 9. Movimiento — Platformer expresivo

### Controles base

| Input | Acción |
|---|---|
| A / ← | Caminar izquierda |
| D / → | Caminar derecha |
| Espacio / W / ↑ | Saltar |
| Espacio en aire | **Doble salto** (Fragmento de Identidad agrega saltos extra) |
| Espacio contra pared | **Wall jump** (rebote diagonal) |
| LeftShift / K | **Dash** horizontal (intangible 0.18s) |
| S / ↓ | Agacharse / reducir hitbox |
| S + Espacio sobre plataforma amarilla | **Drop-through** |
| E | Interactuar |
| ESC | Pausa / Inventario Emocional |

### Detalles técnicos (game feel)

- **Salto variable:** mantener Espacio incrementa la altura hasta un máximo. Soltar antes hace un "jump cut".
- **Coyote time (0.12s):** puedes saltar hasta 0.12s después de salir de una plataforma.
- **Jump buffer (0.15s):** si presionas Espacio justo antes de aterrizar, el salto se ejecuta en cuanto tocas el piso.
- **Aceleración/fricción:** no se usa velocidad directa — hay rampa de aceleración y deceleración que hace el movimiento pesado pero preciso.
- **Wall slide:** al estar pegado a la pared y cayendo, la gravedad se reduce a la mitad.
- **Dash reset:** el dash recarga al tocar el suelo o la pared.

### Variaciones por estado emocional

El controller del Mateo acepta un `speedMultiplier` y `extraJumps` que modifican su física. Los niveles aplican estos valores para que el *movimiento* sea parte del mensaje:

- **Nivel 1 (Ansiedad):** velocidad base al 110% — Mateo camina apurado y tenso. El **Enfoque recarga 50% más lento** (ver §9.c).
- **Nivel 3 (Depresión):** velocidad base al 65% mientras el "peso emocional" esté por encima de 50%. El salto también se reduce. Se recupera usando Anclas Sensoriales. Con peso > 50% el **máximo de Enfoque baja a 2** chispas.
- **Nivel 5 (Integración):** velocidad al 100%, pero con un rastro de luz cálida al correr y un ligero bob en el idle. **Enfoque recarga 30% más rápido**.

### Costo de las acciones (Enfoque)

Las habilidades básicas son **gratis siempre**. Las habilidades avanzadas consumen **chispas de Enfoque** (sistema completo en §9.c). Esta tabla es la fuente única de verdad — cualquier cambio aquí se propaga al resto del documento.

| Acción | Costo |
|---|---|
| Caminar / saltar simple / agacharse / wall slide / interactuar / drop-through | **0 chispas** |
| **Doble salto** | 1 chispa |
| **Dash** | 1 chispa |
| **Wall jump** (impulso) | 1 chispa |
| **Stomp sobre enemigo** | 0 (y devuelve **+1 chispa** como premio) |

**Sin chispas, el jugador puede seguir jugando** — solo no puede usar habilidades avanzadas hasta recargar. Caminar grounded recarga 1 chispa cada 1.2s. Esto evita softlocks: siempre puedes volver atrás caminando.

---

## 9.b Aliento — sistema de vida

**Concepto:** "Aliento" es la vida de Mateo. No se mide en HP numérico ni hay barra UI — son **Fragmentos de Aliento** visuales que viven en el sprite.

### Reglas

- **Default:** 4 fragmentos al empezar el juego.
- **Máximo:** 7 fragmentos (Raíz Cálida +1, Perdón +2, galleta de Memoria Viva +1 permanente).
- **Pérdida de fragmento:**
  - Contacto con enemigo / hazard estándar: **−1 fragmento** + iframes 1s + knockback breve.
  - Pit gris (caída a vacío "blando"): **−1 fragmento**, respawn al borde.
  - **Hazard duro** (lava conceptual, espinas, abismo negro): **Desvanecerse instantáneo** (ver §13.c). Siempre marcado con tinte rojo claro y partícula de advertencia.
- **Recuperación:**
  1. **Santuario** (§13.b) — heal completo.
  2. **Ancla Sensorial** (N3) — restaura 1 fragmento además de bajar peso.
  3. **Power-up "Suspiro Profundo"** — restaura 2 fragmentos al activarlo.
  4. **Té de Tilo** (item de inventario, §11.b) — restaura 1 fragmento. Se usa con tecla H. Stack máx 3.
  5. **Luz Cálida** (§11.b) — restaura 1 fragmento al máximo.
  6. **Galleta de Memoria Viva** (N5) — sube el máximo de Aliento permanentemente +1.
- **NO hay regeneración pasiva por tiempo.** El heal es siempre intencional. La curación es un acto, no un segundo plano.

### Visual diegético

- **NO hay barra de HP en pantalla.**
- Aliento se muestra como un **halo de luz** alrededor del sprite de Mateo, con N "respiraciones" pulsantes (1 pulso por fragmento).
- A 1 fragmento restante: el halo cambia a tono rojo apagado y pulsa más rápido. Frame de tensión visual.
- Al recibir daño: flash rojo en el sprite (no fullscreen) + 0.3s de freeze frame + zoom súbito de cámara + screen shake suave.
- Al curarse: partículas doradas espiralan al sprite + tono cálido en el halo + suspiro audible.

---

## 9.c Enfoque — costo del movimiento avanzado

**Concepto:** Enfoque es la "respiración mental" de Mateo. Es el recurso que cuesta hacer **cualquier acción avanzada de movimiento**. Es la metáfora central del refinamiento v2.1: cuando estás ansioso, tienes menos margen para reaccionar; cuando estás en paz, te alcanza para más.

### Reglas

- **Capacidad:** 3 chispas por defecto. Sube a 4 con el fragmento de identidad **Aura de la Púa**. Sube a 5 con **Última Guitarra**.
- **Costo de acciones:** ver tabla en §9.
- **Recarga (de menor a mayor cadencia):**
  - +1 chispa cada **1.2s** caminando grounded sin tomar daño.
  - +1 chispa al **stomp** a un enemigo.
  - +1 chispa al **recoger un Destello**.
  - **Recarga total instantánea** al tocar una **Luz Cálida** o usar un **Santuario**.
- **Modificadores por nivel:**
  - **N1 (Ansiedad):** recarga **50% más lenta** (1.8s entre chispas).
  - **N2 (Memoria):** recarga normal.
  - **N3 (Depresión):** recarga normal con peso < 30%; **el máximo baja a 2** con peso > 50% hasta usar una Ancla.
  - **N4 (Confrontación):** recarga normal pero con drenaje −1 chispa al recibir un Flash de Recuerdo.
  - **N5 (Integración):** recarga **30% más rápida** (0.85s).
- **Sin chispas no hay softlock:** caminar y salto simple son siempre gratis. La arquitectura del nivel garantiza que toda zona crítica tenga una ruta de retirada caminable.

### Visual diegético

- **NO hay barra de stamina UI.**
- Enfoque se muestra como **2-5 chispas blancas** que orbitan suavemente alrededor del sprite de Mateo.
- Al gastar una chispa: la chispa se desvanece hacia el centro del sprite (el aliento "se gasta hacia adentro"). SFX corto de exhalación.
- Al recargar: la chispa se enciende desde dentro y empieza a orbitar. SFX corto de inhalación.
- Cuando la barra está vacía: 1 chispa parpadea muy tenue indicando que la siguiente recarga está en camino. Mateo cambia a animación `Exhausted` en idle (postura ligeramente encorvada).

---

## 9.d Peso Emocional — refinado y global

**Concepto:** el peso emocional ya no es exclusivo del N3. Existe en TODOS los niveles pero se comporta distinto. Es la **niebla acumulativa** que sigue al sprite cuando las cosas se acumulan.

### Reglas

- **Visual:** una niebla gris-azul sutil que envuelve el sprite. Más densa a más peso. NO hay barra UI excepto en N3 (donde es central y se muestra como medidor vertical opcional).
- **Sube cuando:**
  - Tomas daño (+5%).
  - Fallas una Pregunta de La Sombra de forma evasiva (+10%).
  - Estás más de 8s quieto en zona hostil (+1%/s).
  - Recoges un "recuerdo doloroso" en N3 (+10%).
- **Baja cuando:**
  - Usas una **Ancla Sensorial** (−15%).
  - Te sientas en una **Banca** ≥ 5s (−10%).
  - Encuentras una **Memoria positiva** o **Reflejo cálido** (−5%).
  - Activas un **Santuario** (a 0%).
  - Tocas una **Luz Cálida** (−20%).
- **Efectos por umbral:**

| Peso | Efecto |
|---|---|
| 0–30% | Sin efecto. Niebla casi invisible. |
| 31–60% | Enfoque recarga 25% más lento. Niebla visible. |
| 61–90% | Máximo de Enfoque −1. El sprite tiembla en idle. |
| 91–100% | (Solo posible en N3 sin Anclas) Sprite arrastra los pies, salto −25%, doble salto bloqueado. **NO mata.** Es la metáfora de "no puedes moverte". La única salida es caminar muy despacio hasta una Ancla. |

- **N1, N2, N4, N5** rara vez pasan de 30% — el peso es ambiental y limpia rápido. **N3** sí lo lleva al límite, y la "Banca de 60s" del N3 lo limpia a 0%.

---

## 10. Combate no convencional

Mateo **no tiene arma**. El "combate" ocurre de tres formas:

1. **Esquiva.** Salto, doble salto, dash, wall-jump. La mayoría de los enemigos se evitan con movilidad. Recordar: cada doble salto / dash / wall jump cuesta 1 chispa de Enfoque (§9.c) — las esquivas no son gratis y eso obliga al jugador a leer los patrones en lugar de spamear.
2. **Jump stomp.** Caer sobre un enemigo con velocidad vertical negativa lo derrota y rebota a Mateo (estilo Mario). Funciona con Walkers y Flyers ligeros. **El stomp es la única acción ofensiva que devuelve recurso:** +1 chispa de Enfoque al ejecutar. Esto premia el riesgo activo sobre la evasión total.
3. **Power-ups conceptuales.** Cada power-up cambia las reglas del encuentro (ralentiza, revela, protege, ataca un área). Solo 1 power-up activo a la vez.

### Debuffs (lo que los enemigos aplican)

| Debuff | Origen | Efecto | Duración |
|---|---|---|---|
| Tunnel Vision | Ecos (N1) | Viñeta oscura + velocidad 60% | 4s |
| Controles Invertidos | Voces (N2) | A ↔ D | 3s |
| Pasos de Plomo | Pesos (N3) | Salto al 50% + velocidad 70% | 5s |
| Flash de Recuerdo | Fragmentos Rotos (N4) | Pantalla blanca | 1s |

**Regla de simultaneidad:** solo 1 debuff activo a la vez. Uno nuevo reemplaza al anterior. El escudo del power-up *Escudo de Respiración* consume el debuff entrante sin aplicarlo.

## 11. Power-ups (Objetos conceptuales)

Gestionados por **Object Pooling** (requisito UPN). Solo 1 power-up activo a la vez — uno nuevo reemplaza al anterior. Se obtienen recogiendo el objeto del nivel **o** comprando con Destellos al Eco Amable (§12).

**Marcado:** *MVP* = Sprint 0 (ya implementado o crítico para el prototipo). *F2* = Fase 2 (post-prototipo).

| Power-Up | Nivel | Marcado | Efecto | Duración |
|---|---|---|---|---|
| **Latido Calmado** | 1 | MVP | Ralentiza Ecos en un radio de 6u al 40% | 10s |
| **Memoria Clara** | 1 | MVP | Revela coleccionables cercanos + pulso visual | 8s |
| **Suspiro Profundo** ★ | 1-5 | MVP | **Restaura +2 fragmentos de Aliento** al activarlo. Único power-up de heal directo. | Instantáneo |
| **Escudo de Respiración** | 1-5 | MVP | Ignora el próximo golpe/debuff | Hasta impacto |
| **Foco de Claridad** | 2 | F2 | Indica con aura de color la respuesta correcta a las Voces | 20s |
| **Silenciador** | 2 | F2 | Neutraliza Las Voces en un radio | 12s |
| **Burbuja de Aire** | 3 | F2 | Permite saltar sobre el vacío gris del Abismo | 30s |
| **Corriente Cálida** | 3 | F2 | Reduce peso emocional 25% + velocidad al 100% | 8s |
| **Luz Interior** | 3 | F2 | Ilumina Pesos ocultos, resto del mapa se oscurece | 20s |
| **Escudo de Verdad** | 4 | F2 | Fragmentos Rotos se apartan al acercarse | 15s |
| **Voz Interior** | 4 | F2 | Destaca el espejo correcto con brillo dorado | 10s |
| **Semilla de Luz** | 5 | F2 | Hace crecer una plataforma permanente en una posición marcada | Perm. |
| **Eco de Música** | 5 | F2 | Una melodía revela caminos ocultos del jardín | 25s |

★ **Suspiro Profundo** es nuevo en v2.1. Es el único power-up que cura, y es el motivo por el que la curación a media-room es viable sin trivializar el riesgo (cuesta el slot de power-up, así que renuncias a otra utilidad).

## 11.b Curación, consumibles y Luces Cálidas

El refinamiento v2.1 separa las fuentes de heal en **3 categorías** según frecuencia, costo y diegesis. La regla de oro: la curación nunca es pasiva, siempre es un acto.

### Té de Tilo (consumible de inventario)

- **Qué es:** un consumible permanente del inventario. NO ocupa el slot de power-up.
- **Efecto:** restaura **+1 fragmento de Aliento**. Animación: Mateo se sienta de cuclillas y bebe (~1.5s no cancelables — vulnerable mientras dura, así que no puedes spamearlo en pleno combate).
- **Capacidad:** stack máximo 3. No puedes acumular más.
- **Cómo se obtiene:**
  - 1 garantizado por cada **Banca** activada por primera vez (nunca segunda).
  - Comprar con 10 Destellos al Eco Amable (§12).
  - Drop raro de Memorias Vivas en N5.
- **Tecla:** **H** (nueva) — uso instantáneo.
- **Reglas de uso:** no puedes usar mientras estás aturdido por debuff "Flash de Recuerdo" o "Controles Invertidos". Sí puedes durante "Tunnel Vision" o "Pasos de Plomo".

### Luces Cálidas (objeto ambiental)

- **Qué son:** pequeñas brasas / linternas / faroles / velas distribuidos por los niveles. Visualmente cálidos, con aura dorada y partículas suaves. Compatibles con Light2D.
- **Efecto al tocarlas:**
  - Recarga **100% del Enfoque** (todas las chispas).
  - Restaura **+1 fragmento de Aliento** (sin pasar del máximo).
  - Reduce **−20% Peso Emocional**.
- **Limitadas:** **3 a 5 por nivel**, en posiciones que generan dilemas: "¿la uso antes del salto difícil o me la guardo para después de la zona de Voces?". Una vez tocada, **se apaga permanentemente** (hasta el siguiente respawn por Desvanecerse — entonces se reinician todas).
- **No respawnean** entre Bancas. Sí respawnean al "Desvanecerse" — el ciclo emocional vuelve a empezar.
- **Visual:** brasa flotante con aura dorada de ~1 tile de radio. Se apaga con animación de "exhalación" cuando es consumida.

### Comparativa rápida de fuentes de heal

| Fuente | Cura | Recarga Enfoque | Frecuencia | Costo |
|---|---|---|---|---|
| Té de Tilo | +1 frag | — | A demanda (stack 3) | Slot inventario |
| Suspiro Profundo (power-up) | +2 frag | — | Raro | Slot power-up |
| Ancla Sensorial (N3) | +1 frag | — | Limitado al N3 | Tiempo (5s sostenidos) |
| Luz Cálida (ambiental) | +1 frag | 100% | 3-5 por nivel | Único uso |
| **Banca** | NO cura | — | Frecuente | Gratis |
| **Santuario** | Heal completo | 100% | 2-3 por nivel | Gratis |
| Galleta de Memoria Viva (N5) | Sube max +1 perm | — | 5 únicas en N5 | — |

**Diseño emocional:** las opciones de heal frecuente cuestan tiempo o slot. Las opciones de heal completo (Santuario) son raras. Esto crea ritmo de tensión sin sentirse mezquino: el jugador siempre puede curarse, pero siempre tiene que decidir cómo.

## 12. Destellos de Lucidez (moneda + progresión)

Pequeñas luces que aparecen al superar zonas, clasificar recuerdos, encontrar coleccionables, o tomar decisiones empáticas con La Sombra. Representan **momentos de claridad mental**. En v2.1 son **moneda real** que se gasta en sitios concretos — pero también funcionan como **score acumulado** para Revelaciones pasivas.

### Cómo se obtienen

| Acción | Destellos |
|---|---|
| Superar zona peligrosa evadiendo enemigos | +3 |
| Recoger un coleccionable narrativo | +2 |
| Usar un Ancla Sensorial (N3) | +2 |
| Elección empática con La Sombra (N4) | +3 |
| Mini-interacción con una Memoria Viva (N5) | +3 |
| Completar un nivel | +10 |
| Stomp a un enemigo | +1 |
| Recoger un Destello suelto en el mundo | +1 (y +1 chispa de Enfoque) |

### Bolsa vs. Seguros (refinamiento crítico v2.1)

Los Destellos viven en **dos cubetas** separadas:

- **Bolsa** (parpadeante): los Destellos recién recogidos. Si Mateo se Desvanece (§13.c), **se pierden**.
- **Seguros** (fijo): los Destellos depositados en un Santuario. Sobreviven a cualquier muerte.

**Depositar** ocurre automáticamente al activar un Santuario. Es el gesto narrativo central de v2.1: *"guardar lo que vale para que la crisis no se lo lleve"*.

### Cómo se gastan

Los Destellos NO se gastan en habilidades de movimiento (eso lo hace Enfoque). Solo se gastan en sitios narrativos específicos:

| Donde | Costo | Qué obtienes |
|---|---|---|
| Eco Amable (cualquier nivel) | 5 | 1 power-up del nivel a tu elección |
| Eco Amable | 10 | 1 Té de Tilo (heal item, stack 3) |
| Eco Amable | 25 | Lectura de un diario "perdido" + +1 al contador de diarios |
| Santuario | 50 | 1 Semilla de Luz extra (en N5) |
| Santuario | 100 | Acceso a un cuarto de meditación (mini-flashback opcional) |

Esto da una razón **mecánica** a explorar: los Destellos no son solo score, son recursos. Pero también da una razón **emocional** a depositar: la moneda no es real hasta que la guardas.

### Revelaciones — bonus pasivos por Destellos seguros acumulados

Las Revelaciones siguen existiendo como en v2.0, pero ahora cuentan **solo Destellos depositados** (no bolsa). Esto premia al jugador que cuida sus Santuarios.

- **50 seguros:** El Eco Amable empieza a aparecer en cada nivel y ofrece su comercio.
- **100 seguros:** los niveles ganan detalles de luminiscencia en las capas de parallax — flores en grietas, luces en ventanas, reflejos dorados.
- **200 seguros:** desbloquea el tema musical extendido del menú principal.
- **300 seguros:** desbloquea el **epílogo extendido** (cinemática post-créditos sin requerir el nivel secreto).

## 13. Fragmentos de Identidad (Personalización — Requisito UPN)

Coleccionables especiales que se equipan en el **Inventario Emocional** (menú de pausa o Santuario). **Máximo 3 equipados simultáneamente** — esa es la regla mecánica firme. Coleccionar fragmentos extra es válido para el porcentaje de completitud y para el estado cosmético de "Integración" (ver más abajo). Cada fragmento equipado modifica:

- **Visual:** tint del sprite, sistema de partículas alrededor de Mateo, opcional efecto secundario
- **Stats:** `speedMultiplier`, `extraJumps`, `damageReduction`, `maxFocus`, `maxHealthFragments`

**Coleccionar ≠ Equipar:** puedes tener 7 fragmentos en tu inventario pero solo 3 activos a la vez. Cambiar es libre desde un Santuario, también desde pausa pero con animación de 1.5s "respirando" para evitar swap táctico instantáneo en pleno combate.

### Fragmentos iniciales (Nivel 1-2)

| Fragmento | Visual | Efecto |
|---|---|---|
| **Aura de la Púa** | Tint naranja cálido + partículas de polvo dorado | +5% velocidad |
| **Tinta de Estrellas** | Tint azul celeste + rastro de luces estelares | +1 salto extra |
| **Acero Mental** | Tint gris metálico + sombra definida | -25% daño recibido |

### Fragmentos avanzados (Nivel 3-5)

| Fragmento | Visual | Efecto |
|---|---|---|
| **Ancla del Silencio** | Halo azul tenue | -50% chance de recibir debuff |
| **Raíz Cálida** | Partículas rojo-naranja | +1 fragmento de vida máximo |
| **Voz del Niño** | Bubble de luz infantil | Resetea el dash al hacer wall-jump |
| **Última Guitarra** | Notas musicales flotantes | El dash deja un eco sonoro que aturde enemigos en línea recta |

### Narrativa del sistema

Los Fragmentos representan **partes de Mateo que se rompieron al alejarse Bruno**. Recuperarlos es literalmente *volver a ser él*. El juego lo indica: al tener **los 5 fragmentos narrativos clave en el inventario** (Aura de la Púa, Tinta de Estrellas, Chispa de Atreverse, Voz del Niño, Última Guitarra) — los hayas equipado o no — el retrato de Mateo en el menú principal y en el inventario cambia de silueta a **rostro completo**. Es el estado cosmético "**Integración**". No otorga stats extra. Es solo el reconocimiento visual de que recuperaste lo que importa.

### Habilidades progresivas — desbloqueo diegético (geométrico vs narrativo)

**Mateo no empieza con todas sus habilidades.** Al comienzo del juego solo puede **caminar y saltar**. Las nuevas habilidades vienen por dos vías diegéticas distintas:

- **Diegético geométrico:** la arquitectura del nivel obliga a usar la habilidad para avanzar. Sin fragmento, sin cinemática, sin texto. Crouch, drop-through y wall slide son así — el techo baja, la plataforma es one-way, la pared es muy alta. El jugador *descubre* la mecánica por tentativa.
- **Diegético narrativo:** la habilidad se desbloquea al recoger un **Fragmento de Identidad ligado a un recuerdo emocional**. Hay micro-cinemática (3s), texto flotante con el recuerdo, y una "primera sala test" inmediata. Doble salto, wall jump, dash, dash-reset y eco-sónico son así.

No hay menús de "habilidades desbloqueables" ni tutoriales verbales — el jugador siente que *Mateo recordó cómo se hacía*.

| Habilidad | Nivel | Tipo | Fragmento asociado | Recuerdo que representa |
|---|---|---|---|---|
| Caminar + Saltar | — | default | (default) | Moverse en el mundo |
| **Doble salto** | N1 | narrativo | Aura de la Púa | "Recuerdo estar emocionado por algo" |
| **Crouch + Drop-through** | N1 | geométrico | — | Pasar por debajo de las cosas |
| **Wall slide** | N2 | geométrico | — | Sostenerte cuando te falla el suelo |
| **Wall jump** | N2 | narrativo | Tinta de Estrellas | "Recuerdo haber escrito un cuento sobre estrellas con nombre" |
| **Dash** | N3 | narrativo | Chispa de Atreverse | "Recuerdo haber saltado de un muro sin mirar" |
| **Dash reset en wall-jump** | N4 | narrativo | Voz del Niño | "Recuerdo que me tiraba de árboles sin pensar" |
| **Eco sónico del dash** | N5 | narrativo | Última Guitarra | "Recuerdo la primera canción que escribí" |

**Regla de diseño:** ningún desbloqueo se anuncia con un pop-up verbal. El sistema usa 3 señales diegéticas:
1. **Animación cinemática** al tocar el fragmento (Mateo respira profundo, cambio de luz).
2. **Primer test inmediato** a 5 segundos del desbloqueo — una sala construida específicamente para que solo la habilidad nueva funcione, obligando al jugador a descubrirla por tentativa.
3. **Ícono contextual mínimo** (solo la primera vez, 3 segundos, esquina inferior).

Después del tutorial, la habilidad se asume como conocida y se **reutiliza en cada nivel con variaciones** (repetición con variación — el principio de diseño de Mario y Celeste).

## 13.b Save System de dos tiers — Bancas y Santuarios

El v2.1 introduce una distinción firme entre dos tipos de "punto de descanso". El v2.0 los llamaba a ambos "checkpoints" y eso causaba ambigüedad.

### Tier 1 — Bancas (checkpoints frecuentes)

- **Frecuencia:** cada 3-5 rooms.
- **Activación:** contacto. Banca de madera gris pasa a iluminarse cálida.
- **Funciones:**
  - Registra **respawn point** al sufrir Desvanecerse o caer en pit.
  - **Entrega 1 Té de Tilo** la primera vez que la activas (nunca la segunda).
  - Si te sientas en ella ≥ 5s con la tecla **E** sostenida: **−10% Peso Emocional** + animación contemplativa.
- **Lo que NO hacen:**
  - NO regeneran Aliento ni Enfoque automáticamente.
  - NO guardan en disco.
  - NO permiten reequipar Fragmentos.
- **Visual:** banca gris → amarillo cálido al activarse. Halo dorado tenue 1 tile.

### Tier 2 — Santuarios (save real)

- **Frecuencia:** **2-3 por nivel.** Generalmente: uno cerca del inicio, uno a mitad, uno antes del momento pico.
- **Activación:** interacción con **E sostenido 2s** — gesto de "respirar profundo". El usuario inicia un ritual, no un toque accidental.
- **Funciones (combinadas, todas en una sola interacción):**
  1. **Save persistente real.** El juego escribe a disco. Puedes salir al menú principal y volver al mismo punto exactamente.
  2. **Heal completo.** Aliento y Enfoque al máximo.
  3. **Depositar Destellos.** Toda tu bolsa parpadeante pasa a ser dorado fijo. Inmune a la próxima muerte.
  4. **Reequipar Fragmentos de Identidad** sin penalización de tiempo (a diferencia del menú de pausa).
  5. **Mapa del nivel.** Overlay con rooms exploradas en gris claro, no exploradas como contornos vacíos, secretos encontrados como ★, secretos pendientes como **?** sin posición.
  6. **Lectura de diarios** ya coleccionados.
  7. **Comerciar con Eco Amable** si está cerca (en Santuarios donde el Eco se manifiesta).
- **Inviolabilidad:** La Sombra y los enemigos **no entran en el radio de ~6 tiles** del Santuario. Es un círculo emocionalmente neutro. La música del nivel se silencia y suena un motif corto de guitarra en su lugar.
- **Visual único por nivel:**

| Nivel | Visual del Santuario | Nombre temático |
|---|---|---|
| N1 — Umbral | Estación de radio escolar abandonada con linterna intacta sobre el banco | "La señal" |
| N2 — Archivo | Archivo de cartas de madera con velas encendidas | "El cajón abierto" |
| N3 — Mar Quieto | Flor luminosa gigante flotando con mariposas | "La flor que respira" |
| N4 — Espejos | Espejo intacto sin grieta, con marco dorado | "El reflejo entero" |
| N5 — Jardín | Árbol joven con linternas de papel colgando | "Donde crece" |
| Secreto | (no aplica — el nivel secreto es lineal y único) | — |

### Reglas comunes de las dos tiers

- **Bancas y Santuarios son visibles desde el camino principal.** Nunca ocultos. La exploración se premia con secretos, no con puntos de seguridad.
- **El primer Santuario de un nivel siempre está cerca del spawn** del nivel — el jugador siempre tiene una red de seguridad inmediata.
- **El último Santuario** está antes del momento pico narrativo del nivel, para que el clímax no se pueda perder por una muerte tonta.

## 13.c Desvanecerse — Fail State

El v2.1 introduce un fail state suave que reemplaza el respawn invisible del v2.0. La idea: el jugador **sí puede perder**, pero no se siente castigado — se siente **acompañado en la caída**.

### Trigger

- Aliento llega a 0 (cualquier fuente: enemigo, hazard, pit gris).
- O contacto con un **hazard duro** marcado en rojo (lava conceptual, espinas, abismo negro). Estos infligen Desvanecerse instantáneo, no −1 fragmento.

### Secuencia

1. **t=0s:** Mateo cae lentamente de rodillas en su posición. La música baja a 20% en 0.4s.
2. **t=0.5s:** Fade a blanco suave (1.5s).
3. **t=2s:** Fondo negro. Aparece **una de 6 frases rotativas** centrada, fade in 0.8s:
   - *"Otra vez había demasiado ruido."*
   - *"Cerré los ojos un momento."*
   - *"Mi mente se fue antes que mi cuerpo."*
   - *"No supe qué hacer con todo eso."*
   - *"Era más fácil quedarse quieto."*
   - *"Solo necesitaba respirar."*
   - **No aparece "GAME OVER" en ningún momento.**
4. **t=4s:** Fade a negro.
5. **t=5s:** Fade in en el **último Santuario activado**. Mateo aparece sentado en el Santuario respirando (animación `Sanctuary` 2s).
6. **t=7s:** Aliento y Enfoque al máximo. HUD vuelve. Control devuelto.

### Costo de Desvanecerse

| Recurso | ¿Se pierde? |
|---|---|
| **Destellos en bolsa** (no depositados) | **Sí — perdidos.** |
| **Destellos seguros** (depositados en el Santuario) | No. Intactos. |
| **Bancas activadas** desde el último Santuario | Se "des-activan" (debes re-tocarlas). |
| **Coleccionables** (Recuerdos, Diarios, Anclas, Llaves, Identidades) | **No** — son permanentes. |
| **Tés de Tilo** en inventario | **No** — los conservas. |
| **Power-up activo** | Se pierde. |
| **Debuff activo** | Se limpia. |
| **Peso Emocional** | Vuelve al valor que tenía al entrar al Santuario (no se "premia" ni "castiga"). |

### Sistema de mercy (anti-frustración)

- **3 muertes consecutivas en la misma room:** en el siguiente respawn aparece una **Luz Cálida temporal** en la entrada de la room (en una posición segura, no en el peligro).
- Esta Luz Cálida es opcional. El jugador puede ignorarla y seguir intentando "limpio".
- Si la usa: recarga Enfoque al 100% y restaura **+1 fragmento extra** (sobre el ya-recargado por el Santuario).
- **Texto opcional al pasar cerca:** la Sombra susurra una sola vez: *"Está bien tomarlo. No es trampa."*
- La Luz Cálida temporal **desaparece al salir de la room o al usarla**. Solo aparece una vez por trigger de 3 muertes.

**Por qué no es un Game Over duro:** el juego trata la salud mental con dignidad. Una pantalla de "MORISTE" enseñaría que la respuesta a la crisis es vergüenza. El juego enseña que la respuesta es **regresar al lugar seguro y respirar**. Esa es la lección mecánica.

## 14. Coleccionables

| Tipo | Nivel | Cantidad | Uso |
|---|---|---|---|
| Destellos de Lucidez | Todos | ~80 sueltos + bonus por acción | **Moneda + score** (§12) |
| **Tés de Tilo** ★ | Todos | ~12 (uno por Banca de primer toque + drops) | Heal +1 fragmento (§11.b) |
| **Luces Cálidas** ★ | Todos | 3-5 por nivel (~20) | Recarga total Enfoque + heal +1 + −20% peso (§11.b) |
| Fragmentos de Recuerdo | 1 | 8 | Lore + 5 obligatorios |
| Notas de Voz | 1 | 5 | Lore opcional |
| Objetos Clasificables | 2 | 10 | Puzzle + lore (Guardar / Soltar / Entender) |
| Páginas de Diario | 2 | 6 | Lore opcional |
| Anclas Sensoriales | 3 | 8 | Mecánica + lore + heal +1 |
| Botellas con Mensaje | 3 | 5 | Lore (Bruno) |
| **Llave Oxidada** | 3 | 1 | **Easter egg** — acceso al nivel secreto |
| Reflejos de Espejo | 4 | 8 | Lore |
| **Fragmentos de Identidad** | 1-5 | 7 narrativos + 2 ocultos | Personalización (max 3 equipados) |
| Galletas de Memoria Viva | 5 | 5 | +1 Aliento máximo permanente cada una |
| **TOTAL** | | **~115** | |

★ Categorías nuevas en v2.1.

**Inventario Emocional:** pantalla en el menú de pausa que muestra todos los coleccionables encontrados, el retrato emocional de Mateo, los Tés disponibles (stack), los fragmentos equipables, y el contador de Destellos seguros / en bolsa.

**Porcentaje de completitud** visible al terminar el juego — motivación para rejugabilidad y búsqueda del nivel secreto. Calculado sobre **rooms exploradas + coleccionables encontrados + Santuarios usados + ramas opcionales recorridas**.

---

# IV. DISEÑO DE NIVELES

## Principios de diseño aplicados a todos los niveles

**1. Onboarding diegético ("show, don't tell").**
Cada mecánica nueva se enseña a través de la **arquitectura del espacio**, nunca con tutoriales verbales. Si el jugador necesita aprender a agacharse, el techo baja. Si necesita wall-jump, hay una pared imposible de subir sin él. Si necesita dash, hay un hueco que solo el dash cruza. El jugador descubre por **tentativa guiada**, no por lectura.

**2. Kishōtenketsu por room (introducción → desarrollo → giro → resolución).**
Cada room individual (~30-60 seg de gameplay) sigue una microestructura:
- **Ki** — introduce una idea/mecánica en contexto seguro
- **Shō** — la desarrolla con variación
- **Ten** — añade un twist inesperado (enemigo, hazard, cambio de ritmo)
- **Ketsu** — resuelve permitiendo al jugador demostrar lo aprendido
Es el principio de *Super Mario Bros. 3* y *Celeste*.

**3. Repetición con variación.**
Cada mecánica introducida aparece **en al menos 3 contextos distintos** dentro del mismo nivel y al menos 1 en niveles posteriores. El dash se introduce en N3, pero aparece en N4 como esquiva de Fragmentos Rotos y en N5 como carrera libre.

**4. Arco de habilidades narrativamente vinculado.**
Las habilidades se desbloquean al recuperar Fragmentos de Identidad en momentos emocionales clave. Ver tabla en §13 "Habilidades progresivas".

**5. Ritmo tensión / calma.**
Cada 3-4 rooms intensas vienen 1-2 rooms de respiración (vista panorámica, diálogo, coleccionable tranquilo, banca para sentarse). El jugador nunca debe quedar agotado más de ~2 minutos seguidos.

**6. Fondos de parallax con gameplay implícito.**
Las 4-6 capas de parallax no son decoración. Muestran:
- Eventos del pasado (siluetas de personas en la escuela de N1)
- Movimiento emocional (engranajes acelerando durante una alarma)
- Presencia antagónica (La Sombra caminando en una capa cercana en N4)
- Estado del jugador (color del fondo cambia con el peso emocional en N3)

**7. Duración real y denso.**
Cada nivel dura **25-30 minutos** de gameplay normal, con ~25-32 rooms agrupadas en 3 actos. Un speedrun lo completa en ~10-12 min. Un completionist en ~40-50 min buscando secretos.

**8. Cada nivel tiene al menos un "momento pico" memorable.**
Una secuencia cinematográfica única de 30-60 segundos donde gameplay, narrativa, visual y audio convergen. Son los momentos que el jugador contará cuando hable del juego.

**9. Secretos opcionales recompensan exploración.**
Cada nivel tiene 2-4 rutas secundarias que dan Fragmentos de Recuerdo, Páginas de Diario, o Fragmentos de Identidad extra. Ninguna es obligatoria para terminar el juego — pero el porcentaje final de completitud motiva rejugar.

**10. Muerte nunca castiga de más, pero sí cuenta.**
**Bancas** frecuentes (~cada 3-5 rooms) actúan como respawn point inmediato sin guardar progreso. **Santuarios** (2-3 por nivel) son el verdadero refugio: heal completo, save real, depósito de Destellos, mapa del nivel. El fail state "**Desvanecerse**" (§13.c) te lleva al último Santuario activado y pierdes los Destellos no depositados — pero **no hay pantalla de Game Over**. El juego premia la persistencia, no la perfección. Y el sistema de mercy (Luz Cálida tras 3 muertes consecutivas en una room) garantiza que ningún jugador se quede atorado por orgullo.

## Progresión mecánica global

| Nivel | Habilidad nueva | Mecánica de sistema nueva | Duración |
|---|---|---|---|
| **N1 — Umbral** | Doble salto, crouch, drop-through | Debuffs (Tunnel Vision), Ecos, alarmas, power-ups | 25-30 min |
| **N2 — Archivo** | Wall slide, wall jump, carry | Clasificación de recuerdos, inversión de controles | 25-30 min |
| **N3 — Mar Quieto** | Dash | Peso emocional, anclas sensoriales | 25-30 min |
| **N4 — Espejos** | (consolidación) | Diálogo-combate, plano doble de gameplay | 20-25 min |
| **N5 — Jardín** | Semilla de Luz | Hub-and-spokes, carrera libre | 25-30 min |
| **Secreto** | (ninguna) | Walking simulator narrativo | 10 min |

**Total de gameplay:** ~130-155 minutos (sin secretos), ~180-220 minutos (con exploración completa y secreto).

---

## Nivel 1 — EL UMBRAL (Ansiedad)

| Campo | Detalle |
|---|---|
| **Duración objetivo** | 25-30 min (30 rooms + 2-4 rooms de ramas opcionales) |
| **Emoción** | Desorientación → presión → climax ansioso → silencio |
| **Enemigos** | Los Ecos |
| **Habilidades desbloqueadas** | Salto simple (R1.2) → **Doble salto** (R1.7, *Aura de la Púa*) → Crouch (R1.12, geometría) → Drop-through (R1.18, plataformas amarillas) |
| **Coleccionables** | 8 Fragmentos de Recuerdo, 5 Notas de Voz, 1 Fragmento de Identidad, 1 Cassette oculto, ~3 Tés de Tilo, 4 Luces Cálidas |

### Sistemas activos en el nivel

- **Enfoque:** recarga **50% más lenta** (1.8s entre chispas) — el aire de la ansiedad. Capacidad inicial: 3 chispas.
- **Aliento:** 4 fragmentos de inicio.
- **Peso Emocional:** ambiental, baja por sí solo. Rara vez supera 30%.
- **Bancas:** 3 — R1.9 ("Primer descanso"), R1.22 ("Banca de pizarras"), R1.25 ("Tras la alarma").
- **Santuarios:** **2** — R1.6.A ("La señal" — radio escolar abandonada, en la rama opcional A), R1.27 ("La señal otra vez" — antes del clímax narrativo).
- **Luces Cálidas:** 4 — distribuidas en R1.13, R1.20, R1.24, R1.30.
- **Hazards duros:** ninguno en este nivel. N1 es introducción — todo es −1 fragmento, no Desvanecerse instantáneo.
- **Power-ups disponibles:** Latido Calmado, Memoria Clara, Suspiro Profundo, Escudo de Respiración.

### Ramas opcionales

| ID | Nombre | Acceso | Rooms | Recompensa |
|---|---|---|---|---|
| **N1.A** | Sala de Profesores | Bifurcación visible en R1.5 (puerta de madera con ventana iluminada) | R1.5.A1 → R1.5.A2 → **R1.6.A (Santuario)** | Santuario "La señal" + 1 Página de Diario + 1 Té de Tilo |
| **N1.B** | Cuarto de Mantenimiento | Bifurcación vertical en R1.19 — requiere wall jump (no disponible en N1) | 2 rooms verticales | 1 Fragmento de Recuerdo + 1 Cassette — **acceso solo en backtrack post-N2** |
| **N1.C** | Tras el reloj | Bifurcación oculta en R1.27 detrás del marco del reloj | 1 room | 2 Notas de Voz + 1 Luz Cálida |

### Principio de diseño del nivel

*"Lo cotidiano se vuelve opresivo."* El nivel empieza en el lugar más familiar de Mateo (su aula vacía al amanecer) y termina en un mundo industrial que ya no reconoce. El crescendo es **puramente ambiental** en el Acto I (sin enemigos, sin peligro) y se vuelve **mecánico** en el Acto II cuando aparecen los primeros Ecos.

El nivel funciona como tutorial completo del juego **sin ningún tutorial verbal**. Todos los controles se descubren por tentativa guiada.

### Fondos (parallax, 6 capas)

| Capa | Factor | Contenido | Evolución en el nivel |
|---|---|---|---|
| Sky | 0.02 | Cielo gris industrial con humo | Amanecer frío → mediodía nublado → luz roja de alarma |
| Far | 0.1 | Siluetas de edificios escolares | Se deforman progresivamente, se estiran verticalmente |
| Mid | 0.3 | Engranajes gigantes | Empiezan estáticos, giran durante alarmas |
| Near | 0.65 | Tuberías, cables colgantes, papeles volando | Más densos hacia el final |
| Gameplay | 1.0 | Tilemap principal | — |
| Foreground | 1.25 | Vapor ocasional, polvo | Pasa por delante del jugador |

---

### Acto I — Despertar (rooms 1.1 – 1.9, ~7 min)

**Emoción:** Curiosidad inquieta. **Enseña:** Caminar, saltar, doble salto.

**R1.1 — "El pupitre"**
Mateo duerme con la cabeza sobre un pupitre. El aula está vacía, luz cálida del amanecer entra por ventanas altas. Al dar cualquier input, cinemática corta (2s) de Mateo levantándose. Pared izquierda bloquea el retroceso. **Tarea oculta:** descubrir que A/D camina.

**R1.2 — "El primer escalón"**
Dos pupitres plataforma separados por 2 tiles de hueco. Bajo el hueco hay un "colchón de papeles" (zona suave que revota al jugador arriba, no mata). El jugador intenta avanzar, cae, rebota, descubre solo que Espacio es salto. **Ki-Shō-Ten-Ketsu:** intento → caída → rebote sorpresa → salto exitoso.

**R1.3 — "El pasillo que respira"**
Corredor largo recto. En las ventanas del fondo (parallax far) se ven siluetas de compañeros moviéndose aceleradamente, como si el mundo exterior fuera a velocidad distinta. Ambient: murmullo de clase amortiguado. **Momento de respiración.** Primera Nota de Voz opcional al final: *"...creo que no dormí bien otra vez."*

**R1.4 — "Primer Destello"**
Sala circular con una vitrina rota. Un **Destello de Lucidez** (partícula dorada) flota en el centro. Se recoge caminando por encima. HUD suma +1. Enseña qué son los Destellos sin explicarlo.

**R1.5 — "La altura imposible"** *(bifurcación a Rama N1.A)*
Un Fragmento de Recuerdo visible en una plataforma alta. El jugador intenta salto simple, falla. **Frustración intencional de ~5 segundos.** Justo cuando se rinde, nota una puerta en la pared derecha (camino principal) **y** una segunda puerta de madera con ventana iluminada en la pared izquierda — la entrada a la **Rama N1.A "Sala de Profesores"**. La rama es opcional pero contiene el primer Santuario del juego.

**Rama N1.A — Sala de Profesores (opcional)**
- **R1.5.A1 "Pasillo de avisos"**: corredor corto sin enemigos, paredes cubiertas de avisos amarillentos del último año escolar. 1 Nota de Voz al final.
- **R1.5.A2 "Mesa de exámenes"**: sala con 3 cajas industriales que se pueden empujar (Sokoban suave). El puzzle: poner las 3 cajas sobre 3 marcas en el piso. Al resolverlo, la pizarra del fondo proyecta una sombra que revela una entrada cerrada a otra room.
- **R1.6.A "La señal" — PRIMER SANTUARIO DEL JUEGO**: estación de radio escolar abandonada, micrófono polvoriento, una linterna intacta sobre el banco. El jugador descubre la mecánica de Santuario por tentativa: prompt [E sostenido 2s]. Animación ritual de "respirar profundo" 1.5s. Música del nivel se silencia, motif de guitarra suena, partículas doradas, +Aliento, +Enfoque, depósito automático de Destellos. **Texto flotante muy sutil al salir:** *"Aquí siempre puedes volver."*
- **Recompensa adicional:** 1 Página de Diario + 1 Té de Tilo en el cajón del banco.
- Vuelta al camino principal por una segunda puerta detrás del Santuario que conecta directo con R1.7.

**R1.6 — "La luz dorada"** *(camino principal — solo si NO entraste a la rama A)*
Sala pequeña iluminada por un haz de luz cayendo del techo roto. Sin enemigos. Solo una silueta dorada flotando en el centro.

**R1.7 — "Aura de la Púa" (ANCLA 1)**
Al tocar la silueta, cinemática (3s): Mateo respira profundo, aparece una púa de guitarra en su mano. Música de guitarra suave empieza. Texto flotante: *"Recuerdo estar emocionado por algo."* **Desbloquea doble salto.** Icono [Espacio x2] aparece 3s en la esquina. **No se dice "tienes doble salto"** — el jugador lo descubre en la siguiente room.

**R1.8 — "El primer test"**
Salida con hueco de 4 tiles. Imposible con salto simple. El jugador intenta Espacio, cae, intenta de nuevo, prueba Espacio+Espacio. Descubre. Al otro lado, puede volver opcionalmente a R1.5 a recoger el Fragmento que no alcanzaba.

**R1.9 — "Primer descanso" (Banca 1)**
Banca de madera gris. Al tocarla, fade a amarillo. Micro-animación. **Sin texto.** El jugador lo aprende contextualmente. Por ser primera activación, **entrega 1 Té de Tilo** que aparece flotando junto a la banca. Si el jugador se sienta con E sostenido 5s, animación de respirar y −10% Peso Emocional.

---

### Acto II — Deformación (rooms 1.10 – 1.22, ~13 min)

**Emoción:** Presión, confusión creciente, paranoia. **Enseña:** Crouch, drop-through, esquivar Ecos, usar power-ups.

**R1.10 — "La primera sombra"**
Corredor normal. Al fondo (parallax near-layer), un Eco pasa caminando en dirección opuesta. Susurra: *"...no vas a llegar..."*. Se desvanece al acercarte. Mateo puede ver pero no tocar. Introducción visual al enemigo.

**R1.11 — "Eco sobre la plataforma"**
Primer Eco en el gameplay layer, patrullando una plataforma ALTA que el jugador NO necesita pisar. La ruta principal va por abajo. Enseña: "el enemigo existe, es evitable si no subo".

**R1.12 — "El techo que baja"**
Corredor con techo descendente visible. El jugador llega a una sección donde el techo es de solo 1.5 tiles de alto. Imposible pasar de pie. **Descubre Abajo = crouch** por necesidad.

**R1.13 — "Agacharse bajo un Eco" (Luz Cálida 1)**
El mismo corredor bajo ahora tiene un Eco patrullando arriba. El jugador debe agacharse y caminar debajo. Siente el peligro sobre su cabeza. Al final del corredor, en una alcoba lateral, una **Luz Cálida** (brasa flotante con halo dorado). El jugador puede tomarla aquí o seguir y guardarla mentalmente para más adelante — el primer dilema de recurso del juego.

**R1.14 — "Primera cinta"**
Cinta transportadora (plataforma móvil horizontal) corta, en piso, sin peligros. Lleva al siguiente cuarto. Introducción suave.

**R1.15 — "Cinta contra Eco"**
Cinta larga que empuja hacia un Eco patrullando. El jugador debe correr contra ella (A si la cinta va hacia D) o saltar sobre el Eco (jump-stomp implícito).

**R1.16 — "Latido Calmado"**
Caja industrial con un power-up cyan flotando encima. Al recoger, los 2 Ecos del cuarto se ralentizan visiblemente (animación a 40%). El jugador aprende el efecto mirando. Se llama *Latido Calmado* (texto flotante breve).

**R1.17 — "Pasillo de pizarras"**
Corredor estrecho con pizarras en el fondo llenas de ecuaciones tachadas y frases ansiosas: *"No sirvo para esto", "Otra vez tarde", "Nadie me nota"*. Ecos estáticos pegados a las pizarras como decoración — no dañan, son ambiente. Un Fragmento de Recuerdo en esquina alta (requiere doble salto de precisión).

**R1.18 — "Las plataformas amarillas"**
Primera one-way platform (amarilla clara). El jugador la atraviesa saltando desde abajo (paso libre). Para descender, intenta saltar y no puede — prueba Abajo + Espacio — **drop-through descubierto por tentativa**.

**R1.19 — "Torre de cajas"**
Laberinto vertical de cajas industriales con 3 Ecos patrullando a distintas alturas. Combinación de doble salto + drop-through + evasión. Primer reto integrado. Cassette oculto detrás de una caja que parece pared.

**R1.20 — "Primera alarma" (Luz Cálida 2)**
Cuarto amplio. De repente: sonido de alarma industrial. Luz roja pulsante. Los 2 Ecos del cuarto aceleran x1.8 por 5s. El parallax mid-layer muestra engranajes girando muy rápido. **Enseña ciclo de peligro.** El jugador aprende a esperar la alarma o sprintar durante ella. Una **Luz Cálida** colgada del techo en un punto que se accede solo con doble salto preciso — recompensa el riesgo de subir durante la alarma.

**R1.21 — "El pasillo de papel"**
Momento de respiración. Pasillo decorado con papeles de exámenes volando en foreground. Tres Notas de Voz opcionales en esquinas.

**R1.22 — "Banca de pizarras + vista panorámica" (Banca 2)**
Banca de descanso. Al sentarse, la cámara hace pull-back 3s para mostrar el horizonte del parallax: ya se ve la Sala del Reloj en la distancia y la fábrica exterior. El jugador siente que hay mucho más por delante. Primera activación: entrega 1 Té de Tilo.

---

### Acto III — El Eco Más Fuerte (rooms 1.23 – 1.30, ~10 min)

**Emoción:** Climax ansioso → silencio. **Enseña:** Consolidación total + secuencia narrativa.

**R1.23 — "Fábrica exterior"**
Primera vista al exterior real. Cielo industrial rojizo. Tuberías gigantes como plataformas. Cintas en múltiples niveles. Post-processing viñeta activa pulsando con el latido. **Sala oculta:** caminando contra una pared que parece sólida (engranaje decorativo), se entra a una sala con 3 Notas de Voz. **Pista de parallax:** la silueta de un papel volando en el parallax near-layer aparece "atravesando" la pared falsa exactamente en la posición correcta — el jugador atento la lee.

**R1.24 — "Circuito de cintas 1/3" (Luz Cálida 3)**
Plataformas móviles en dos direcciones contrarias. Ecos entre ellas. El jugador aprende a medir saltos con cintas en movimiento. Una **Luz Cálida** flotando entre las cintas — debe sincronizar para tocarla sin caer.

**R1.25 — "Circuito de cintas 2/3" (Banca 3)**
Alarma dispara al entrar. Se combina presión de tiempo + plataformas móviles + evasión. **Banca** al final ("Tras la alarma"). Primera activación: entrega 1 Té de Tilo.

**R1.26 — "Circuito de cintas 3/3"**
Más complejo: cintas verticales (plataformas que suben/bajan). Requiere salto con timing. Fragmento de Recuerdo opcional accesible solo con wall-jump... pero el jugador aún no lo tiene. Se queda como gancho para rejugabilidad post-N2.

**R1.27 — "Sala del Reloj — entrada" (SANTUARIO 2 + Rama N1.C)**
Puerta masiva se abre. Dentro: una sala enorme con un reloj mecánico dominando todo el fondo. Manecilla visible girando lenta. Audible: tic... tac... tic... tac... (cada 1.2s). Plataformas móviles sincronizadas al ritmo. **Antes** de las plataformas, en una alcoba lateral iluminada, está el **segundo Santuario "La señal otra vez"** — la misma estación de radio del Santuario 1, ahora con la luz parpadeando. Última oportunidad de guardar antes del momento pico narrativo. **Detrás del marco del reloj** (pared falsa visible solo si la cámara enfoca la esquina superior derecha — pista por parallax) está la entrada a la **Rama N1.C "Tras el reloj"** con 2 Notas de Voz y 1 Luz Cálida extra.

**R1.28 — "Sala del Reloj — travesía"**
Plataformas que aparecen/desaparecen con el tic. Ecos que se congelan en tic y avanzan en tac. El jugador debe cruzar leyendo el ritmo del reloj. Si cae a un pit, aterriza en colchón bajo — no muere, pero pierde posición. **Momento pico visual.**

**R1.29 — "Sala del Reloj — cima opcional"**
Un Fragmento de Recuerdo muy alto. Requiere varios dobles saltos sincronizados al tic exacto. Recompensa a jugadores cuidadosos.

**R1.30 — "La sala del teléfono"** *(MOMENTO PICO NARRATIVO + Luz Cálida 4)*
Habitación pequeña silenciosa tras la Sala del Reloj. Toda la música se apaga de golpe. Un teléfono sobre una mesa de madera. Luz tenue. Junto al teléfono, una vela encendida — la **última Luz Cálida** del nivel. Mateo camina automáticamente hacia el teléfono (camera lock, input horizontal bloqueado).

**Secuencia del teléfono:**
1. Texto flotante: *"47 mensajes sin leer de Bruno."*
2. Los mensajes aparecen uno por uno, uno encima del otro, apilándose: *"¿estás bien?"* ... *"contesta porfa"* ... *"solo dime algo"* ... *"ya no sé qué hacer"* ... *"te extraño"* ... *"no tienes que decir nada"* ... *"solo quería que supieras"* ...
3. Silencio total 2s.
4. Aparecen 2 opciones grandes: **[E] RESPONDER** / **[Q] NO PUEDO**
5. Ambas válidas. Ambas suman +10 Destellos. La elección se guarda en `GameManager` y afecta 3 Reflejos de Espejo y una Pregunta de La Sombra en N4.
6. 3 segundos de silencio absoluto tras elegir.
7. Animación: el teléfono se desintegra en partículas de luz. En su lugar flota un **Fragmento de Recuerdo dorado** (el más importante del nivel).
8. Al recogerlo: flash blanco, música de guitarra suave empieza, HUD suma +10.

**R1.31 — "La salida"** *(bonus, post-momento pico)*
Corredor corto iluminado por luz cálida. Menos ruido. Post-processing calma la viñeta. Una puerta dorada al final. Mateo atraviesa. **Fade a blanco.** Pantalla de resumen: destellos del nivel, % de completitud, secretos encontrados.

---

### Secretos del Nivel 1

Etiqueta: 🌿 = Rama opcional / 🔍 = Pista en parallax / ⏳ = Backtrack futuro / 💫 = Acción específica

| # | Ubicación | Categoría | Contenido |
|---|---|---|---|
| S1 | R1.5.A — Rama Sala de Profesores | 🌿 | **Santuario "La señal"** + Página de Diario + Té de Tilo |
| S2 | R1.19 — detrás de caja empujable | 💫 | Cassette oculto |
| S3 | R1.23 — pared falsa (pista: papel del parallax) | 🔍 | Sala con 3 Notas de Voz |
| S4 | R1.26 — plataforma alta | ⏳ | Fragmento de Recuerdo (requiere volver en N2 con wall-jump) |
| S5 | R1.27 — detrás del marco del reloj (Rama N1.C) | 🌿🔍 | 2 Notas de Voz + 1 Luz Cálida |
| S6 | R1.29 — ruta del reloj en cima | 💫 | Fragmento de Recuerdo |
| S7 | R1.19 — Rama N1.B Cuarto de Mantenimiento | ⏳ | Fragmento de Recuerdo + Cassette (requiere wall jump de N2) |

### Diálogos clave (textos flotantes ambientales)

- R1.1: *"...otra mañana."*
- R1.3: *"Conozco este lugar. Estoy aquí todos los días. Y aun así me siento perdido."*
- R1.7: *"Recuerdo estar emocionado por algo. ¿Cuándo fue?"*
- R1.17: *"No sé por qué corro si la puerta siempre está igual de lejos."*
- R1.27: *"El reloj no marca horas. Marca las veces que no dije nada."*
- R1.30: *"(47 mensajes sin leer de Bruno.)"*

### Transición al Nivel 2

Fade a blanco → texto flotante: *"No todo lo que cargas te pesa. Algunas cosas te sostienen."* (línea del Eco Amable, introducida antes de su primera aparición) → fade in al Archivo.

---

## Nivel 2 — EL ARCHIVO (Memoria y Confusión)

| Campo | Detalle |
|---|---|
| **Duración objetivo** | 25-30 min (28 rooms + 3-5 rooms de ramas opcionales) |
| **Emoción** | Nostalgia → confusión → ternura incómoda → aceptación |
| **Enemigos** | Las Voces (zonas de inversión estáticas) |
| **Habilidades desbloqueadas** | Carry (R2.3, E para cargar objetos) → Wall slide (R2.9, geometría) → **Wall jump** (R2.12, *Tinta de Estrellas*) |
| **Coleccionables** | 10 Objetos Clasificables, 6 Páginas de Diario, 1 Cassette oculto, 1 Fragmento de Identidad, ~3 Tés de Tilo, 4 Luces Cálidas |

### Sistemas activos en el nivel

- **Enfoque:** recarga **normal** (1.2s entre chispas). Desde el momento en que recoges Tinta de Estrellas, el wall jump consume chispas como cualquier otra acción avanzada.
- **Aliento:** 4-5 fragmentos.
- **Peso Emocional:** ambiental, sube ligeramente al recibir debuff "Controles Invertidos" (+3% por cada inversión completa).
- **Bancas:** 3 — R2.7 ("Banca del puente"), R2.20 ("Banca del archivo"), R2.26 ("Banca de las cajas").
- **Santuarios:** **3** — R2.9 ("El cajón abierto" con primer Eco Amable), R2.18 ("Cajón de cartas" — opcional, en rama N2.B), R2.25 ("Banca de la maqueta").
- **Luces Cálidas:** 4 — distribuidas en R2.13, R2.16, R2.22, R2.27.
- **Hazards duros:** ninguno. Las caídas son a oscuridad pero te respawnean en la última plataforma sin daño.
- **Power-ups disponibles:** Foco de Claridad, Silenciador, Suspiro Profundo, Escudo de Respiración.

### Ramas opcionales

| ID | Nombre | Acceso | Rooms | Recompensa |
|---|---|---|---|---|
| **N2.A** | Estanterías Inferiores | Bifurcación visible en R2.16 (escotilla en el suelo) | 3 rooms Sokoban con tarjetas | Fragmento de Identidad **"Sabiduría del Silencio"** |
| **N2.B** | Mapas Translúcidos | Bifurcación visible en R2.17 (puerta translúcida con planos arquitectónicos) | 2 rooms de plataformas giratorias | **Santuario "Cajón de cartas"** + 2 Páginas de Diario |
| **N2.C** | Galería de Voces | Secreto puro de parallax — silueta de una boca en el parallax mid-layer indica una pared falsa en R2.19 | 1 room | Cassette oculto + Frase Oculta |

### Principio de diseño del nivel

*"Las cosas que guardas te pesan o te sostienen."* El nivel está **literalmente fragmentado**: islas flotantes suspendidas en la oscuridad de la memoria. Para avanzar, Mateo debe clasificar recuerdos — y la clasificación modifica el mundo (nuevas islas emergen según elijas Guardar / Soltar / Entender). Es el único nivel con **mecánica de clasificación + inversión de controles**, y el primero que introduce el wall-jump.

**Twist de diseño:** el wall-jump no se enseña con un salto imposible sin él — se enseña narrativamente. El jugador **sabe que puede deslizarse** por las paredes (wall slide forzado), pero no sabe que puede impulsarse. Cuando recoge el Fragmento "Tinta de Estrellas", un recuerdo específico de infancia (trepar un árbol) es el que desbloquea el salto. El primer test viene inmediato, como en N1.

### Fondos (parallax, 5 capas)

| Capa | Factor | Contenido |
|---|---|---|
| Sky | 0.0 | Negro absoluto con partículas doradas flotando (como archivo de memoria) |
| Far | 0.15 | Siluetas muy difusas de muebles, estanterías, lámparas |
| Mid | 0.4 | Planos arquitectónicos translúcidos flotando, a veces girando |
| Near | 0.7 | Papeles volando, tarjetas de archivo, fotos sueltas |
| Gameplay | 1.0 | Islas de madera y metal con tablones puente |

---

### Acto I — Entrar al Archivo (rooms 2.1 – 2.9, ~9 min)

**Emoción:** Curiosidad suave, nostalgia cálida. **Enseña:** Carry, primera clasificación, wall slide.

**R2.1 — "La entrada dorada"**
Transición desde N1. Mateo aterriza suavemente en la primera isla — tras la tensión del Nivel 1, la calma es casi abrumadora. Luz dorada, partículas flotando. Vista del fondo completo 3 segundos. **Momento de respiración post-N1.**

**R2.2 — "La púa"**
Isla pequeña con una mesa de madera. Sobre la mesa: una **púa de guitarra** brillante. Es un **Objeto Clasificable**. Al acercarse, prompt [E].

**R2.3 — "Cargar"**
Al presionar E, Mateo levanta la púa sobre su cabeza (sprite visible). Sigue caminando normal pero con el objeto. Enseña **carry** por acción. El Mateo con objeto camina un 15% más lento — sutil pero perceptible.

**R2.4 — "Las tres mesas"**
Cámara se abre a una plataforma amplia con 3 pedestales:
- **GUARDAR** — baúl dorado, luz cálida
- **SOLTAR** — hoguera azul, llamas suaves
- **ENTENDER** — escritorio con lupa, luz verde

El jugador debe caminar hasta uno y presionar E de nuevo. La púa se deposita. Cada elección tiene una animación única de 4-5 segundos:
- *Guardar:* la púa se envuelve en luz y el baúl la absorbe.
- *Soltar:* la púa arde con llama azul, se disuelve, Mateo suspira visible.
- *Entender:* la púa se desarma en piezas en el aire y un flashback visual muestra una escena (Mateo tocando guitarra de niño).

Tras clasificar, el piso tiembla. Nuevas islas emergen del fondo. **Feedback claro:** clasificar genera mundo.

**R2.5 — "Segunda isla"**
Nuevas plataformas accesibles. Otro objeto: una **foto vieja**. Otra clasificación. Diferentes animaciones.

**R2.6 — "Primera Voz en el fondo"**
Corredor entre islas. En el parallax near-layer, una boca flotante (Una Voz) susurra distorsionado: *"eso no pasó así..."*. No es peligrosa aún — está lejos.

**R2.7 — "Puente estrecho"**
Tablón de madera de 1 tile de ancho entre dos islas. Salto preciso. Primer test de precisión.

**R2.8 — "La pared alta"**
Al final de un corredor, una pared de 6 tiles de alto bloquea el paso. Arriba se ve una plataforma con un Objeto Clasificable. **Imposible con doble salto solo.** El jugador intenta, falla, se pega a la pared — **descubre wall slide** (sprite de Mateo se queda pegado con gravedad reducida). Aprende: "puedo colgarme, pero no subir... todavía."

**R2.9 — "El cajón abierto" — SANTUARIO 1 + Primer Eco Amable**
Isla segura con un archivo de cartas de madera con velas encendidas — el primer Santuario del Nivel 2. El **Eco Amable** aparece por primera vez (silueta traslúcida cálida). Diálogo cinematográfico corto:
*"Hola, Mateo. Siéntate un momento."*
Mateo se sienta junto al archivo 3 segundos. El Eco Amable le entrega un power-up de regalo: **Foco de Claridad** + abre su comercio de Destellos (puedes intercambiar acá).
*"No todo lo que guardas te pesa. Algunas cosas te sostienen."*
Santuario activado: heal completo, depósito automático de Destellos, save persistente real, mapa del nivel disponible. Última oportunidad de reequipar fragmentos antes del Acto II.

---

### Acto II — Las Voces (rooms 2.10 – 2.20, ~12 min)

**Emoción:** Confusión, inversión, persistencia. **Enseña:** Voces, inversión de controles, wall jump, puzzle ambiental.

**R2.10 — "Primera Voz en gameplay"**
Una Voz (boca flotante) en el gameplay layer crea una zona visible de niebla púrpura (3 tiles ancho). Al pisarla: **controles invertidos 3 segundos**. El jugador lo siente. Al pasar, los controles vuelven. Frase que flota: *"Eso no pasó así."*

**R2.11 — "Dos Voces contiguas"**
Dos zonas de Voz consecutivas sin espacio entre ellas. El jugador debe aprovechar la inversión — si va hacia la derecha, debe presionar A. **Enseña que la inversión es manejable si la anticipas.**

**R2.12 — "Tinta de Estrellas" (ANCLA 2)**
Isla aislada con una sola figura flotante azul. Al tocarla, cinemática: partículas estelares azules rodean a Mateo. Texto flotante: *"Recuerdo haber escrito un cuento sobre estrellas con nombre. Me trepaba a los árboles y les ponía nombres desde arriba."* **Desbloquea wall jump.** Icono [⇧+⟶] 3s.

**R2.13 — "Test de wall jump"**
Sala vertical con 3 paredes alternas (zigzag). Imposible sin wall jump. El jugador descubre que al deslizarse por una pared puede saltar hacia la opuesta. **Ki-Shō-Ten-Ketsu completo en una room.**

**R2.14 — "El Muro de Mapas"**
Pared muy alta cubierta de mapas antiguos en el parallax. Wall jumps encadenados. En una esquina del muro, una **Página de Diario** accesible solo con wall jump preciso. Primer coleccionable que recompensa dominio.

**R2.15 — "La Voz alta"**
Zona de Voz colocada en el MEDIO de una secuencia de wall jumps. El jugador debe hacer wall jumps con controles invertidos. Exige concentración.

**R2.16 — "Depósito de Cajas — Acto I"**
Sala horizontal con cajas industriales apiladas. Cada caja tiene una etiqueta: "vacío", "tristeza", "ira", "alegría", "silencio". El jugador puede empujarlas (caminar contra ellas). Puzzle estilo Sokoban simplificado: usa las cajas para formar escalones.

**R2.17 — "Depósito de Cajas — Acto II"**
Dos Voces dificultan el puzzle. Empujar cajas con controles invertidos es difícil pero posible. Una Página de Diario oculta dentro de una caja "silencio".

**R2.18 — "La Radio Eterna"**
Plataforma amplia con una radio gigante en el centro (2 tiles alto). Al interactuar con distintas partes (antena, dial, parlante), suenan fragmentos de una melodía incompleta — es la **canción de Mateo y Bruno**. Pistas: el dial giratorio revela un coleccionable escondido en el parallax mid-layer (sale a gameplay cuando giras el dial correctamente).

**R2.19 — "El Cassette oculto"**
Sala secreta detrás de un plano arquitectónico translúcido (hay que mirar bien el parallax near-layer para ver la entrada). Contiene el **Cassette oculto** — solo reproducible en el nivel secreto con Bruno.

**R2.20 — "Banca del archivo + vista panorámica" (Banca 2)**
Isla circular con una banca. Al sentarse, la cámara hace pull-back mostrando el horizonte completo del nivel: se ve la **Mesa Final** en la distancia + el borde del abismo de N3. Respiración visual. Primera activación: 1 Té de Tilo.

---

### Acto III — La Mesa Final (rooms 2.21 – 2.28, ~9 min)

**Emoción:** Decisión, catarsis, transición. **Enseña:** Tests integrados + momento narrativo.

**R2.21 — "Circuito de Objetos"**
Ruta con 4 Objetos Clasificables repartidos: audífonos viejos, carta sin abrir, medalla escolar, dibujo infantil. Cada uno en una sub-isla con un micro-puzzle distinto (wall jump, salto temporizado, evasión de Voces). El jugador elige qué orden recogerlos.

**R2.22 — "El pasillo sin gravedad"**
Solo el fondo: planos arquitectónicos flotando como plataformas giratorias. Uno debe saltar plataformas que rotan. Visual único, bello.

**R2.23 — "El Eco Amable — segunda aparición"**
Isla con fogata. Diálogo corto:
*"¿Recuerdas cuándo dejaste de tocar la guitarra? No te juzgo. Solo quiero saber si lo recuerdas."*
Entrega un power-up: **Silenciador** (neutraliza Voces en un radio por 12s).

**R2.24 — "Gran laberinto de Voces"**
Sala con 5 Voces distribuidas. Uso obligatorio del Silenciador. El jugador planifica ruta. Si lo hace bien, hay un Fragmento de Recuerdo al final.

**R2.25 — "El momento de la banca"**
Isla pequeña con una banca de taller mecánico. Mateo puede sentarse (E). 15 segundos de silencio. Aparece un flashback del parallax: Mateo de 12 años construyendo una maqueta con Bruno. **No da recompensa mecánica — solo es un momento.**

**R2.25 — "El momento de la banca" (Banca 3 + SANTUARIO 3)**
Isla pequeña con una banca de taller mecánico **junto a un altar de cartas iluminado por velas**. El altar es el tercer Santuario del nivel ("Banca de la maqueta") — última oportunidad de save antes del momento pico narrativo. Si Mateo se sienta en la banca (no en el Santuario), 15 segundos de silencio + flashback de Mateo de 12 años con Bruno construyendo una maqueta. Si activa el Santuario, save real + heal completo + depósito de Destellos.

**R2.26 — "Última ruta" (Banca pequeña refundida en R2.25)**
Corredor final con los 3 últimos Objetos Clasificables más difíciles de alcanzar. Requiere wall jumps precisos + esquivar 2 Voces simultáneas.

**R2.27 — "La Gran Mesa"** *(MOMENTO PICO NARRATIVO)*
Sala circular amplia. 3 mesas enormes: Guardar / Soltar / Entender. Objetos Clasificables finales: **púa de Mateo, foto de Bruno y Mateo, nota sin abrir**.

El jugador debe clasificar los 3. Cada clasificación dispara un flashback visual en el parallax completo (la escena inunda el fondo 5 segundos):
- *Púa → Guardar:* Mateo en su cuarto, guitarra colgada.
- *Púa → Soltar:* Mateo regalando la púa a Bruno hace años.
- *Púa → Entender:* Mateo y Bruno componiendo juntos la canción.
- (Similar para foto y nota — 9 posibles mini-cinematicas en total.)

La combinación elegida se guarda en `GameManager` y afecta 2 Memorias Vivas del Nivel 5 (cómo hablan con Mateo).

**R2.28 — "El puente dorado"**
Tras la Gran Mesa, un puente de luz se forma hacia el borde de la última isla. Camino al abismo del Nivel 3. Fade a gris profundo.

---

### Secretos del Nivel 2

Etiqueta: 🌿 = Rama opcional / 🔍 = Pista en parallax / ⏳ = Backtrack futuro / 💫 = Acción específica

| # | Ubicación | Categoría | Contenido |
|---|---|---|---|
| S1 | R2.14 — cima del Muro de Mapas | 💫 | Página de Diario (wall jump preciso) |
| S2 | R2.16 — Rama N2.A Estanterías Inferiores | 🌿 | Fragmento de Identidad **"Sabiduría del Silencio"** |
| S3 | R2.17 — caja "silencio" | 💫 | Página de Diario |
| S4 | R2.17 — Rama N2.B Mapas Translúcidos | 🌿 | **Santuario "Cajón de cartas"** + 2 Páginas de Diario |
| S5 | R2.18 — dial de la Radio | 💫 | Fragmento de Recuerdo |
| S6 | R2.19 — Rama N2.C Galería de Voces (pista parallax) | 🌿🔍 | **Cassette oculto** + Frase Oculta |
| S7 | Si clasificas los 10 objetos todos "ENTENDER" | 💫 | Bonus narrativo: el Eco Amable aparece en R2.28 con un diálogo único |

### Diálogos clave

- R2.9: *"No todo lo que guardas te pesa. Algunas cosas te sostienen."* (Eco Amable)
- R2.10: *"Eso no pasó así."* (Voz)
- R2.12: *"Recuerdo haber escrito un cuento sobre estrellas con nombre."*
- R2.18: *"La radio nunca termina la canción. Igual que yo."*
- R2.25: *"Hay cosas que no recuerdo haber guardado. Pero ahí están, ocupando espacio en mí."*
- R2.27: *"No tiré nada. Solo dejé de mirar. Y de alguna forma eso fue peor."*

### Transición al Nivel 3

Puente dorado → borde del archivo → Mateo se asoma al abismo gris → cae en slow motion mientras el parallax se desatura progresivamente a monocromo → fade in al Nivel 3 con silencio ambiental.

---

## Nivel 3 — EL MAR QUIETO (Depresión)

| Campo | Detalle |
|---|---|
| **Duración objetivo** | 25-30 min (26 rooms + 4-6 rooms de ramas opcionales) |
| **Emoción** | Vacío → peso → rendición → silencio sanador → ascenso |
| **Enemigos** | Los Pesos (adherentes, pasivos, no persiguen) |
| **Habilidades desbloqueadas** | **Dash** (R3.11, *Chispa de Atreverse*) + sistema de **Peso Emocional** activo central |
| **Coleccionables** | 8 Anclas Sensoriales, 5 Botellas con Mensaje, **1 Llave Oxidada** (easter egg), ~3 Tés de Tilo, 5 Luces Cálidas |

### Sistemas activos en el nivel

- **Enfoque:** recarga **normal** con peso < 30%. **Máximo baja a 2** chispas con peso > 50%. Volver a 3 al usar Anclas.
- **Aliento:** las Anclas Sensoriales ahora **curan +1 fragmento** además de bajar peso (refinamiento v2.1).
- **Peso Emocional:** el sistema vive aquí. **Único nivel donde llega a 91-100%.** La Banca de 60s lo limpia a 0.
- **Bancas:** 2 — R3.10 ("Pequeña banca del descenso"), R3.25 (refundida con la Torre de Anclas).
- **Santuarios:** **3** — R3.2 ("La flor que respira" — el primer Santuario del nivel, ya en la cama-plataforma del inicio), R3.16.A ("La flor del fondo" — opcional, en la rama del Fondo Luminoso), R3.18 ("La flor renacida" — tras la Banca de 60s, en pleno ascenso, refleja el cambio del nivel).
- **Luces Cálidas:** 5 (más que cualquier otro nivel — N3 es el más duro emocionalmente y el sistema lo compensa).
- **Hazards duros:** **1** — el "abismo total" del Acto I (un agujero negro marcado con tinte rojo en R3.7) que infligiría Desvanecerse. **El doc lo marca explícitamente** porque es la única excepción al "no game over" del nivel.
- **Power-ups disponibles:** Burbuja de Aire, Corriente Cálida, Luz Interior, Suspiro Profundo, Escudo de Respiración.

### Ramas opcionales

| ID | Nombre | Acceso | Rooms | Recompensa |
|---|---|---|---|---|
| **N3.A** | Fondo Luminoso (refinada) | Bifurcación visible en R3.10 — solo desbloquea con peso < 50% (la luz dorada solo brilla cuando estás "ligero") | **4 rooms** (antes 1) | **Llave Oxidada** + Santuario "La flor del fondo" + Fragmento dorado + 2 Botellas |
| **N3.B** | Las Algas | Bifurcación lateral en R3.14 — corriente que jala — debe usar dash para entrar | 3 rooms con corrientes | 3 Botellas con Mensaje + 1 Té de Tilo |
| **N3.C** | El piano roto | Bifurcación contemplativa en R3.21 — pista visual: notas musicales flotando del lado izquierdo del parallax | 1 room | **Cura permanente +1 Aliento máximo** + Frase Oculta |

### Principio de diseño del nivel

*"Bajar es a veces el único camino."* Es el **único nivel principalmente descendente** — Mateo cae por un abismo gris por los primeros dos actos, y asciende en el tercero tras el momento de la banca. Es también el único nivel donde **el sistema de juego penaliza al jugador emocionalmente pero nunca lo mata** — el peso emocional reduce velocidad y altura de salto, pero nunca hay un game over por "estar triste". La única salida es la paciencia.

**Twist de diseño emocional:** el momento pico del nivel **no es una pelea ni un puzzle** — es **sentarse en una banca durante 60 segundos reales sin hacer nada**. Es el anti-gameplay como clímax. Celeste lo hace en el Capítulo 7; SHATTER lo pone antes.

**El dash como acto de voluntad:** narrativamente, el dash representa *"atreverte a moverte cuando todo pesa"*. Mecánicamente, los frames de invulnerabilidad del dash permiten atravesar Pesos sin adherirlos — una metáfora del "empujar a pesar de".

### Fondos (parallax, 5 capas — cambian de color post-banca)

| Capa | Factor | Pre-banca | Post-banca |
|---|---|---|---|
| Sky | 0.0 | Gris denso uniforme | Azul profundo con estrellas tenues |
| Far | 0.1 | Niebla monocromo | Amanecer distante naranja-morado |
| Mid | 0.3 | Siluetas de muebles flotando (cama, escritorio, piano) | Iguales, con luces tenues en ventanas |
| Near | 0.6 | Partículas grises cayendo como ceniza | Partículas azul pálido flotando arriba |
| Gameplay | 1.0 | Plataformas grises semi-translúcidas | Plataformas con venas de luz azul |

El cambio es **gradual post-banca** — cada nueva room tiene un 10% más de saturación que la anterior.

---

### Acto I — Caer (rooms 3.1 – 3.10, ~9 min)

**Emoción:** Vacío, desorientación, peso creciente. **Enseña:** Peso emocional, Anclas Sensoriales, Pesos.

**R3.1 — "La caída"**
Transición desde N2. Mateo cae verticalmente 5 segundos en slow motion a través del parallax (las capas se pasan rápidamente). Aterriza suavemente en una cama flotante (primera plataforma).

**R3.2 — "La primera cama" — SANTUARIO 1 ("La flor que respira")**
Plataforma: una cama deshecha flotando. Mateo puede caminar sobre ella. HUD: **aparece la barra de Peso Emocional** (vertical, lado izquierdo, empieza en 30%) — único nivel con barra UI explícita. Sin explicación. El jugador la ve y aprende mirando. Junto a la cama, una **flor luminosa gigante con mariposas orbitando** — el primer Santuario del Nivel 3. El jugador, golpeado por la transición pesada del N2, encuentra inmediato refugio. Activa con [E sostenido 2s]: heal completo, depósito automático, save real. Peso baja a 0% por primera y posiblemente única vez en el Acto I.

**R3.3 — "Descenso suave"**
Plataformas descendentes en zigzag. Sin enemigos. El jugador siente que Mateo camina más lento. Enseñanza implícita: *"algo pesa ya"*.

**R3.4 — "Primer Peso"**
Una esfera oscura flotante en el camino. El jugador la evita instintivamente. Si no, se adhiere al sprite de Mateo (partícula negra pegada a la espalda) y aplica **Pasos de Plomo** 5s (altura de salto al 50%).

**R3.5 — "Primera Ancla Sensorial"**
Plataforma con una **flor brillante** en el centro. Prompt [E sostenido 5s]. Al hacerlo, Mateo se sienta en flor de loto — animación única contemplativa de 5 segundos. Partículas de colores estallan en pantalla. **Peso −15% + +1 fragmento de Aliento** (refinamiento v2.1: las Anclas ahora también curan). Frase flotante: *"Olor a... algo que recuerdo."*

**R3.6 — "El primer recuerdo doloroso"**
Un Fragmento de Recuerdo gris-rojo flota en una plataforma. Al recogerlo: flash rápido de una imagen (Mateo solo en la cafetería). **Peso +10%.** Enseñanza: *"recoger no siempre es ganar"*.

**R3.7 — "La corriente empuja" (HAZARD DURO)**
Plataforma amplia con tiles de "corriente gris" que empujan a Mateo hacia un pit. **Único hazard duro del nivel:** el pit central tiene tinte rojo y partículas de advertencia — caer ahí inflige **Desvanecerse instantáneo**, no daño parcial. Los pits laterales son grises (estándar, −1 fragmento). El jugador aprende a leer la diferencia visual de hazard. Debe caminar contra la corriente — lento con peso alto. Enseña que el peso afecta todo.

**R3.8 — "Dos Pesos contiguos"**
Pasillo angosto con 2 Pesos flotando en patrón oscilante. El jugador debe sincronizar movimiento.

**R3.9 — "Segunda Ancla — Tacto"**
Roca rugosa en una plataforma. Al interactuar E sostenido, Mateo pasa la mano por la textura. Pantalla tiembla suave. Peso -15%. Frase: *"Áspero. Real. Sigo aquí."*

**R3.10 — "Pequeña banca del descenso" (Banca 1 + bifurcación a Rama N3.A)**
Plataforma amplia con una banca pequeña — **Banca**, no Santuario. Pull-back de cámara muestra el abismo completo: arriba el archivo dorado, abajo la niebla descendiendo, a un lado se ve una **luz tenue dorada al final del abismo**. Esa luz es la entrada al **Fondo Luminoso (Rama N3.A)** — pero **solo es visible si el peso del jugador está < 50%**. Si el peso es alto, el jugador ve solo niebla. Es la única bifurcación del juego que requiere "estar bien" emocionalmente para ver. Primera activación: 1 Té de Tilo.

---

### Acto II — La Banca (rooms 3.11 – 3.17, ~10 min)

**Emoción:** Atrevimiento, catarsis, silencio. **Enseña:** Dash, uso integrado, anti-gameplay como clímax.

**R3.11 — "Chispa de Atreverse" (ANCLA 3)**
Plataforma aislada en el centro del abismo. Luz blanca cegadora. Al tocar el Fragmento: Mateo da un paso adelante, vibra. Texto flotante: *"Recuerdo haber saltado de un muro sin mirar."* **Desbloquea dash.** Icono [⇧Shift] 3s.

**R3.12 — "El primer hueco dashable"**
Inmediatamente: un hueco de 6 tiles. Doble salto no alcanza. Shift = dash. **Descubrimiento forzado.** El jugador cruza.

**R3.13 — "Dash entre Pesos"**
Corredor con 3 Pesos flotando muy cerca unos de otros. El jugador descubre que **el dash atraviesa Pesos sin adherirlos** (los frames de invulnerabilidad). Metáfora visual clara.

**R3.14 — "Circuito de dashes"**
4 huecos consecutivos. Dash → plataforma → dash. Se reseta al tocar piso. Test de ritmo.

**R3.15 — "Tercera Ancla — Oído"**
Plataforma con un caracol marino. E sostenido. Mateo se lo pone al oído. Suena una canción de cuna infantil. Peso -15%. Frase: *"Mi mamá me cantaba esto cuando tenía cinco años."*

**Rama N3.A — Fondo Luminoso (refinada en v2.1, 4 rooms)**
Solo accesible con **peso < 50%** desde R3.10. Zona descendente extra con plataformas doradas semi-translúcidas. Tiles de arena con ruinas sumergidas en el fondo.

- **R3.10.A1 "Descenso dorado"**: corredor descendente con Pesos pasivos. Sin combate. Música del nivel cambia a un motif suave.
- **R3.10.A2 "Las ruinas"**: plataformas con grabados antiguos en las paredes. Pista visual: las grabados forman una constelación.
- **R3.10.A3 "La flor del fondo" — SANTUARIO 2**: el segundo Santuario del nivel. Una flor luminosa más grande aún que la del inicio, con luciérnagas. Heal completo, depósito de Destellos, save real, y permite **comerciar Destellos** con el Eco Amable que se manifiesta solo aquí.
- **R3.10.A4 "El altar de la llave"**: cámara final con la **Llave Oxidada** flotando en un altar de arena. Al recogerla: cinemática 4s — la llave hace eco al moverla. **Recompensas:** Llave + Fragmento de Recuerdo dorado raro + 2 Botellas con Mensaje.

Vuelta al camino principal por una corriente ascendente que regresa al Acto II.

**R3.16 — "El umbral de la banca"** *(antes era el Fondo Luminoso — ahora es transición visual al momento pico)*
Plataforma intermedia entre R3.15 ("Tercera Ancla — Oído") y R3.17 ("La Banca"). Sin enemigos. Solo una luz tenue al frente y un susurro lejano. El jugador siente que algo importante viene. Es una room de respiración de 10 segundos antes del momento de la banca de 60s.

**R3.17 — "La Banca"** *(MOMENTO PICO — ANTI-GAMEPLAY)*
Plataforma amplia con una banca de madera simple. Al acercarse, Mateo camina automáticamente hacia ella y se sienta (animación lenta de 4 segundos). **Todo el HUD desaparece.** La música se apaga. Solo queda el sonido del viento suave.

**60 SEGUNDOS REALES. Sin poder moverse. Sin input.**

Durante los 60 segundos:
- **0-10s:** silencio absoluto.
- **10-20s:** el parallax far-layer empieza a cambiar lentamente de gris a azul.
- **20-35s:** una nota de piano suena cada 3 segundos. Flashbacks muy tenues en el fondo de momentos felices de Mateo.
- **35-50s:** el piano se convierte en melodía incompleta. El cielo pasa a madrugada.
- **50-60s:** crescendo sutil. Amanecer en el parallax.
- **60s:** texto en pantalla — *"Descansar también es avanzar."*
- La barra de peso va a **0%**. HUD vuelve. Mateo se levanta. El mundo está coloreado.

**Nota de diseño:** el jugador puede hacer ALT+TAB y volver. El tiempo corre en segundo plano. No hay skip. Es intencional — es un momento de confianza entre juego y jugador. *"Si te atreves a quedarte quieto aquí, te regalo un mundo nuevo."*

---

### Acto III — Subir (rooms 3.18 – 3.26, ~9 min)

**Emoción:** Ligereza, esperanza tenue, esfuerzo cálido. **Enseña:** Combinación dash + wall-jump + doble salto en ascenso.

**R3.18 — "El primer paso del ascenso" — SANTUARIO 3 ("La flor renacida")**
Tras la banca, el jugador camina unos pasos. La vista se invierte: ahora hay que subir. Primera wall-jump en el abismo ya coloreado. Justo al inicio del ascenso, la flor del Santuario inicial **reaparece transformada** — más grande, más viva, ahora con luciérnagas. Es el tercer Santuario del nivel ("La flor renacida"), explícitamente paralela visual del primer Santuario para subrayar el cambio narrativo. Heal completo, depósito, save real.

**R3.19 — "Cuarta Ancla — Vista"**
Prisma de cristal flotante. E sostenido. Un arcoiris tenue cruza el parallax completo por 3 segundos. Peso se mantiene en 0. Frase: *"No todos los colores se fueron. Solo estaban cubiertos."*

**R3.20 — "Primera Botella de Bruno"**
Botella con papel enrollado. Al interactuar: *"Yo también estuve aquí. Respira. — B."* **Hint importante:** Bruno también cayó en este abismo en algún momento.

**R3.21 — "Corriente contracorriente"**
Plataformas móviles que suben y bajan. Las corrientes empujan hacia abajo. Debe combinar dash + doble salto para ascender.

**R3.22 — "Quinta Ancla — Memoria"**
Foto flotante. E sostenido. Flash en pantalla completa de un recuerdo feliz (Mateo + Bruno + otro amigo riendo). Peso se mantiene en 0. Frase: *"Éramos ruidosos. Éramos bellos."*

**R3.23 — "Segunda Botella"**
*"La primera vez que alguien me preguntó si estaba bien, no supe qué responder. Perdón por no haberte preguntado antes. — B."*

**R3.24 — "Fragmento Raíz Cálida"**
Plataforma dorada aislada. Fragmento de Identidad **"Raíz Cálida"** — +1 fragmento de vida máximo. No requiere cinemática — el jugador ya aprendió el ritual.

**R3.25 — "Torre de anclas" (Banca 2 refundida)**
Último ascenso: torre vertical de 6 plataformas, cada una con una Ancla Sensorial pequeña que funciona como **mini-banca** (respawn point sin save real). Wall jumps en cadena. La última Ancla, en la cima, es una **Banca completa** (entrega 1 Té de Tilo si es primera vez).

**R3.26 — "La superficie"**
Mateo emerge del abismo a una plataforma iluminada por amanecer completo. Cámara hace pull-back final para mostrar el abismo abajo, azul y lleno de luces tenues. **"Descansar también es avanzar"** reaparece como texto fantasma 2 segundos. Fade a blanco. Transición a N4.

---

### Secretos del Nivel 3

Etiqueta: 🌿 = Rama opcional / 🔍 = Pista en parallax / ⏳ = Backtrack futuro / 💫 = Acción específica

| # | Ubicación | Categoría | Contenido | Requisito |
|---|---|---|---|---|
| S1 | R3.10 — Rama N3.A Fondo Luminoso (R3.10.A4) | 🌿 | **Llave Oxidada** + Fragmento dorado + 2 Botellas + Santuario "La flor del fondo" | Peso < 50% al pasar por R3.10 |
| S2 | R3.14 — Rama N3.B Las Algas | 🌿 | 3 Botellas con Mensaje + 1 Té de Tilo | Dash desbloqueado |
| S3 | R3.21 — Rama N3.C El piano roto (pista parallax: notas musicales en izq) | 🌿🔍 | **Cura permanente: +1 Aliento máximo** + Frase Oculta | — |
| S4 | Completar Acto III con peso = 0 | 💫 | Cinemática extra: Mateo en la superficie con silueta de Bruno al lado | Mantener peso 0 todo el ascenso |

### Diálogos clave

- R3.5: *"Olor a... algo que recuerdo."*
- R3.6: *"No tengo miedo. No tengo nada. Y eso es lo más extraño que he sentido."*
- R3.11: *"Recuerdo haber saltado de un muro sin mirar."*
- R3.17 (banca): *"Descansar también es avanzar."*
- R3.20: *"Yo también estuve aquí. Respira. — B."*
- R3.23: *"Perdón por no haberte preguntado antes."*

### Transición al Nivel 4

Mateo camina por la superficie iluminada. A lo lejos, un edificio circular con ventanas espejadas. Se acerca. Su propio reflejo lo mira desde una ventana — pero el reflejo **no lo imita**. Fade a rojo oscuro.

---

## Nivel 4 — ESPEJOS ROTOS (Confrontación)

| Campo | Detalle |
|---|---|
| **Duración objetivo** | 22-28 min (26 rooms + 3-4 rooms de ramas opcionales) |
| **Emoción** | Inquietud → reconocimiento → miedo → aceptación → ternura |
| **Enemigos** | Fragmentos Rotos (orbitales), La Sombra (paralelo + directa) |
| **Habilidades desbloqueadas** | Ninguna de movimiento nuevo (consolidación total) + **"Voz del Niño"** fragmento (dash reset en wall jump) + **mecánica de Diálogo-combate** |
| **Coleccionables** | 8 Reflejos de Espejo, 1 Fragmento de Identidad, 4 Frases Ocultas, ~3 Tés de Tilo, 4 Luces Cálidas |

### Sistemas activos en el nivel

- **Enfoque:** recarga normal pero **drenaje −1 chispa al recibir Flash de Recuerdo** (los Fragmentos Rotos) — el debuff te quita aire mental.
- **Aliento:** la consolidación. Mateo llega bien equipado. 5-6 fragmentos esperados.
- **Peso Emocional:** sube +10% por **Pregunta evasiva** con La Sombra. Una respuesta honesta o empática lo deja igual.
- **Bancas:** 3 — R4.8 ("Banca de la galería"), R4.18 ("Banca del pasillo sin espejos"), R4.22 (con Acero Mental).
- **Santuarios:** **2** — R4.4 ("El reflejo entero" — antes de la primera Pregunta), R4.20 ("Banca de la sala vacía" — antes de la Confrontación final).
- **Luces Cálidas:** 4 — distribuidas en R4.6, R4.13, R4.17, R4.21.
- **Hazards duros:** **1** — la "fosa de cristales" en R4.13 (espinas de espejo). Marcado en rojo.
- **Power-ups disponibles:** Escudo de Verdad, Voz Interior, Suspiro Profundo, Escudo de Respiración.

### Ramas opcionales

| ID | Nombre | Acceso | Rooms | Recompensa |
|---|---|---|---|---|
| **N4.A** | Sala de Espejos Quebrados | Bifurcación visible en R4.11 — un espejo entero entre los rotos sirve de portal | 2 rooms con puzzle de "caminar atrás avanza" | Fragmento alternativo de **"Voz del Niño"** + Frase Oculta |
| **N4.B** | Cuarto del Trompo (refinada) | Sub-zona ya existente — refinada de 1 a 3 rooms | 3 rooms | Página con historia de infancia + Reflejo de Espejo extra |
| **N4.C** | Detrás del marco | Secreto puro de parallax — un marco vacío en el parallax far-layer indica una pared falsa en R4.7 | 1 room | Frase Oculta + 1 Té de Tilo |

### Principio de diseño del nivel

*"Encontrarte a ti mismo es el miedo más grande."* N4 es un **nivel de consolidación mecánica** — no introduce habilidades nuevas de movimiento — pero es **el nivel más denso narrativamente**. Todo lo que aprendiste se usa ahora en secuencias que exigen precisión mientras la historia escala. El twist de diseño es la **mecánica del plano doble**: La Sombra existe tanto en el parallax near-layer (donde camina imitando/anticipando a Mateo) como ocasionalmente en el gameplay layer durante las Preguntas.

**Mecánica única de N4 — "Diálogo-combate":**
Durante secuencias específicas, La Sombra hace una pregunta. Aparecen 3 opciones flotantes en pantalla mientras el gameplay sigue activo. Mateo debe **esquivar Fragmentos Rotos** durante 10-15 segundos **mientras elige la respuesta**. Las opciones desaparecen si no eliges a tiempo (se elige "silencio" por default). No es punitivo — cada opción tiene valor. Es el único momento del juego donde combate y narrativa son la misma cosa.

### Fondos (parallax, 6 capas — con gameplay implícito en la capa 3)

| Capa | Factor | Contenido |
|---|---|---|
| Sky | 0.0 | Oscuridad roja profunda, muy sutil |
| Far | 0.1 | Marcos de espejos gigantes en perspectiva |
| Mid | 0.35 | Fragmentos de espejo cayendo lentamente como nieve |
| **Near — GAMEPLAY IMPLÍCITO** | 0.7 | **La Sombra camina imitando/anticipando a Mateo.** A veces va más rápido, a veces se detiene y lo mira. |
| Gameplay | 1.0 | Plataformas plateadas y espejos grandes decorativos |
| Foreground | 1.3 | Reflejos ocasionales que pasan por delante |

---

### Acto I — La Galería (rooms 4.1 – 4.9, ~8 min)

**Emoción:** Inquietud creciente, reconocimiento. **Enseña:** Fragmentos Rotos, mecánica de La Sombra en parallax, primera Pregunta.

**R4.1 — "La entrada sin reflejo"**
Sala inicial. Un espejo gigante en la pared del fondo. Mateo camina frente a él. **El reflejo está vacío — no hay Mateo en el espejo.** Inquietud inmediata. Cámara hace un zoom sutil al espejo.

**R4.2 — "El reflejo que llega tarde"**
Segundo espejo. El reflejo aparece pero con 1 segundo de retraso. El jugador camina, el reflejo camina 1 segundo después. Pesadilla sutil.

**R4.3 — "El reflejo que va en otra dirección"**
Tercer espejo. El reflejo camina en dirección opuesta. Al llegar al borde del espejo, desaparece.

**R4.4 — "La Sombra en el parallax" — SANTUARIO 1 ("El reflejo entero")**
Corredor largo. En la capa near-background, se ve una silueta caminando **en paralelo a Mateo**, a veces adelantándolo, a veces quedándose atrás. **No interactúa aún.** El jugador la ve por primera vez. Inquietud. **Antes** del corredor, una alcoba lateral contiene un **espejo intacto sin grieta con marco dorado** — el primer Santuario del Nivel 4. Refugio narrativo: el único espejo que no engaña en todo el nivel. Heal completo, depósito, save real. La Sombra **no aparece** mientras el jugador esté dentro del radio del Santuario.

**R4.5 — "Primeros Fragmentos Rotos"**
Pasillo con 3 Fragmentos de espejo orbitando un centro. El jugador debe cronometrar paso entre las órbitas. Si toca uno: debuff **Flash de Recuerdo** (pantalla blanca 1s + imagen flash de un recuerdo feliz de Bruno).

**R4.6 — "Circuito de órbitas"**
4 orbitadores diferentes a distintas velocidades. Camino sinuoso. Test de timing.

**R4.7 — "Salida a la Galería"**
Pasillo recto con marcos de espejos gigantes en el fondo. La Sombra en el parallax ahora camina **al lado mismo de Mateo**. Mira al jugador directamente por 2 segundos. Se desvanece.

**R4.8 — "Banca de la galería" (Banca 1)**
Banca. Al tocarla, aparece un texto flotante — la Sombra susurra por primera vez desde el parallax: *"Si no intentas nada, nadie puede decepcionarte."* Primera activación: 1 Té de Tilo.

**R4.9 — "La Pregunta 1"** *(primera mecánica de Diálogo-combate)*
Cuarto amplio. La Sombra se materializa **en el gameplay layer** frente a Mateo. Camera lock suave. 6 Fragmentos Rotos empiezan a orbitar alrededor del jugador. La Sombra habla:
*"Dime algo. ¿Por qué contestaste el teléfono?"* (si respondiste en N1)
O: *"¿Por qué no contestaste? ¿Por qué lo dejaste esperando?"* (si no respondiste)

Aparecen 3 opciones flotantes:
- **[1] Porque tenía miedo.** (Honesta)
- **[2] Porque no quería sentirme peor.** (Evasiva)
- **[3] Porque no sabía qué decir.** (Empática)

Mateo puede moverse y esquivar durante 12 segundos. Las opciones desaparecen si no eliges. La elección desactiva ciertos Fragmentos Rotos en la siguiente room:
- Honesta: desactiva 50% de los Fragmentos de R4.10
- Evasiva: desactiva 0% — la room sigue completa
- Empática: desactiva 75%
- Silencio (no elegir): desactiva 25%

La diferencia **mecánica** es clara, pero ninguna opción es "la correcta" — la honesta y la empática son ambas válidas.

---

### Acto II — El Puente de Reflejos (rooms 4.10 – 4.17, ~8 min)

**Emoción:** Descubrimiento propio, puzzle visual, reconocimiento. **Enseña:** Plataformas condicionales por cámara, segunda Pregunta, uso de todas las habilidades.

**R4.10 — "Consecuencia de la Pregunta 1"**
La sala está llena de Fragmentos Rotos orbitando (cantidad según elección anterior). Test de supervivencia. Banca al final.

**R4.11 — "El Puente de Reflejos"** *(mecánica única)*
Una sala amplia donde **las plataformas solo son visibles y sólidas cuando la cámara las enfoca**. El jugador debe **mover la cámara** para hacer aparecer el camino. ¿Cómo se mueve la cámara? Con el **dash** (el dash dispara un boost de look-ahead en la cámara de 2 segundos). Así el jugador descubre una nueva interacción entre dash y el mundo.

**R4.12 — "Puente de Reflejos 2/2"**
Versión avanzada con plataformas que aparecen y desaparecen con delay. El timing del dash importa.

**R4.13 — "La sala sin suelo" (HAZARD DURO + Luz Cálida)**
Sala donde el "piso" es un espejo horizontal. Al caminar sobre él, Mateo se ve reflejado invertido abajo. Fragmentos Rotos cruzan entre Mateo y su reflejo. **Hazard duro:** dos secciones del piso son "espejos quebrados" con espinas — caer ahí inflige Desvanecerse instantáneo. Marcado en rojo. Una **Luz Cálida** flota al final como recompensa por cruzar limpio.

**R4.14 — "Voz del Niño" (ANCLA 4)**
Sala circular con un trompo de madera infantil girando en el suelo. Al tocar: cinemática breve. Texto flotante: *"Recuerdo que me tiraba de los árboles sin pensar que me podía romper."* **Desbloquea:** el dash se resetea cuando haces wall-jump. Esto permite cadenas dash→wall-jump→dash→wall-jump. El jugador lo descubre en la siguiente room.

**R4.15 — "Circuito dash + wall jump"**
Serie de paredes y huecos que requieren cadenas dash+walljump+dash. Inmediato test del fragmento.

**R4.16 — "La Pregunta 2"**
Mecánica igual a R4.9. La Sombra aparece. Pregunta:
*"¿Qué recuerdas de Bruno?"*
Opciones:
- **[1] Su risa.** (Cálida)
- **[2] Nada específico. Solo que estaba.** (Distanciada)
- **[3] Todo. Y eso es lo que duele.** (Honesta)

La elección modifica **qué Reflejos de Espejo coleccionables aparecen** en R4.17-4.19 (cada opción crea una serie distinta de flashbacks visuales). **3 rutas narrativas de coleccionables** dependiendo de la elección — motivo de rejugabilidad.

**R4.17 — "Los Reflejos Visibles"**
Sala amplia con 3 Reflejos de Espejo flotantes (contenido visual depende de R4.16). Cada Reflejo es una micro-escena (3-5 segundos) que juega en el fondo al tocarlo.

---

### Acto III — La Confrontación (rooms 4.18 – 4.26, ~12 min)

**Emoción:** Miedo → reconocimiento → aceptación → ternura. **Enseña:** Consolidación total + momento pico narrativo.

**R4.18 — "El pasillo sin espejos" (Banca 2)**
Corredor largo SIN espejos. Silencio. Solo el sonido del viento distante. Mateo camina sin resistencia. Momento de calma antes de la tormenta. Una **Banca** simple de madera al final del corredor — no hay cinemática, solo descanso. Primera activación: 1 Té de Tilo.

**R4.19 — "Tercera Pregunta"**
La Sombra aparece sin enemigos alrededor esta vez. Mateo no tiene que esquivar. Solo escucha y elige.
*"¿Quién eres sin los demás?"*
Opciones (todas sin "fácil"):
- **[1] No lo sé. Creo que no existo sin ellos.**
- **[2] Soy el que se queda cuando todos se van.**
- **[3] Soy alguien que todavía está aprendiendo quién es.**

Ninguna penaliza. Todas modifican el **diálogo del epílogo del Nivel 5**.

**R4.20 — "Sala vacía con banca" — SANTUARIO 2 ("El reflejo entero II")**
Sala pequeña, limpia, con una banca y un Reflejo de Espejo que muestra a Mateo viéndose a sí mismo **abrazándose**. El jugador puede sentarse en la banca — Mateo se abraza a sí mismo por 5 segundos. **Momento íntimo.** Junto a la banca, otro **espejo intacto con marco dorado** — el segundo Santuario del nivel, **última oportunidad de save antes de la Confrontación final** en R4.24. Heal completo, reequipa fragmentos, deposita Destellos. Texto flotante al activar: *"Antes de cruzar, respira."*

**R4.21 — "Escalada final"**
Torre vertical. Wall jumps + dash + doble salto en cadenas complejas. Fragmentos Rotos en puntos específicos. Última zona de gameplay puro del nivel.

**R4.22 — "Banca con Acero" (Banca 3)**
Plataforma circular con un Fragmento de Identidad: **"Acero Mental"** (−25% daño recibido) sobre una banca. Diálogo implícito: *"Te fortalecí al dejarte escuchar esto."* Banca activable.

**R4.23 — "Entrada a la Sala Circular"**
Una puerta enorme con forma de espejo. Al tocarla: fade a blanco. Cámara se abre lentamente revelando la sala final.

**R4.24 — "La Sala Circular"** *(MOMENTO PICO NARRATIVO)*
Sala circular gigante. **La Sombra parada en el centro.** Todos los espejos alrededor reflejan escenas de la vida de Mateo en el parallax (escuela, guitarra, Bruno, padres, cuarto vacío). Sin Fragmentos Rotos por primera vez. Sin enemigos. Solo ellos dos.

La Sombra habla lento:
*"¿Sabes por qué estoy aquí?"*
*"Aprendí a quedarme quieta para que no te doliera."*
*"Pensé que te estaba protegiendo."*

Pausa. Opción forzada: **[E] Escuchar** (no se puede avanzar sin ello).

La Sombra continúa:
*"Pero me convertí en todas las cosas que no dijiste."*
*"No quiero ser eso."*

Pausa. Aparece la **Pregunta Final**:
*"¿Me dejarías ir contigo? No para que te cuide. Para que nos acompañemos."*

Opciones:
- **[1] SÍ.**
- **[2] NO.**
- **[3] NO LO SÉ.**

**Todas válidas.** Cada una cambia el final del juego:
- *Sí:* La Sombra camina al lado de Mateo desde ahora (sprite compañero visible). Final "Acompañado".
- *No lo sé:* La Sombra se desvanece lentamente pero susurra *"Te escucho, cuando me necesites."* Final "Pendiente".
- *No:* La Sombra se desvanece silenciosa. Final "Solo" — el Nivel 5 tiene un tono diferente, más introspectivo.

**R4.25 — "La salida"**
Tras la elección, la sala se llena de luz cálida. Una puerta verde (primera vez que aparece el color verde en el nivel). Mateo (y La Sombra, si eligió Sí) caminan hacia ella.

**R4.26 — "El umbral verde"**
Fade a verde cálido. Cinemática de transición: un jardín apenas visible en el horizonte. Música de guitarra completa empieza por primera vez. Transición al Nivel 5.

---

### Secretos del Nivel 4

Etiqueta: 🌿 = Rama opcional / 🔍 = Pista en parallax / ⏳ = Backtrack futuro / 💫 = Acción específica

| # | Ubicación | Categoría | Contenido | Requisito |
|---|---|---|---|---|
| S1 | R4.7 — Rama N4.C Detrás del marco (pista parallax) | 🌿🔍 | Frase Oculta + Té de Tilo | — |
| S2 | R4.11 — Rama N4.A Sala de Espejos Quebrados | 🌿 | Fragmento alternativo "Voz del Niño" + Frase Oculta | — |
| S3 | R4.11 — cámara muy lejos | 💫 | Reflejo oculto extra | — |
| S4 | R4.14 — Rama N4.B Cuarto del Trompo (refinada) | 🌿 | Página con historia de la infancia + Reflejo de Espejo | Wall jump alto |
| S5 | R4.20 (banca) | 💫 | Frase Oculta *"Me quería como era"* | Sentarse 10s completos |
| S6 | Final alternativo | 💫 | Sprite compañero de La Sombra visible en todo N5 | Elegir "Sí" en la Pregunta Final |

### Diálogos clave

- R4.8: *"Si no intentas nada, nadie puede decepcionarte."* (Sombra)
- R4.9: *"Dime algo. ¿Por qué contestaste?"* (Sombra)
- R4.14: *"Recuerdo que me tiraba de los árboles sin pensar."*
- R4.19: *"¿Quién eres sin los demás?"* (Sombra)
- R4.24: *"Aprendí a quedarme quieta para que no te doliera."* (Sombra)
- R4.24: *"Pero me convertí en todas las cosas que no dijiste."*
- R4.24: *"¿Me dejarías ir contigo?"*

### Transición al Nivel 5

La puerta verde se abre. Mateo (y opcionalmente La Sombra) camina hacia un jardín vertical. Música de guitarra completa suena por primera vez en el juego. Fade in al Nivel 5.

---

## Nivel 5 — EL JARDÍN (Integración)

| Campo | Detalle |
|---|---|
| **Duración objetivo** | 25-30 min (30 rooms, 5 sub-zonas + plaza + mirador + arco oculto) |
| **Emoción** | Calma → alegría → melancolía → plenitud → dignidad |
| **Enemigos** | Ninguno (La Raíz aparece solo si el jugador no se mueve por 15s — advertencia suave, no daña) |
| **NPCs** | 5 Memorias Vivas, El Eco Amable, La Chispa, (opcional) La Sombra como compañera |
| **Habilidades desbloqueadas** | **Semilla de Luz** (power-up único que crea plataforma permanente), combinaciones libres |
| **Coleccionables** | 5 Fragmentos de Identidad (1 por Memoria), 5 Semillas de Luz (opcionales), páginas finales, **5 Galletas de Memoria Viva** (cada Memoria entrega una al despedirse → +1 Aliento máximo permanente cada una), 5 Luces Cálidas |

### Sistemas activos en el nivel

- **Enfoque:** recarga **30% más rápida** (0.85s entre chispas) — Mateo está en paz.
- **Aliento:** alcanza máximo posible (7 con todos los upgrades). Si recoges las 5 Galletas, **el máximo sube a 9 fragmentos** — sólo posible aquí.
- **Peso Emocional:** baja constantemente. Casi imposible de subir.
- **Bancas:** 2 — R5.1 (bajo el árbol gigante de la Plaza), R5.14 (en la baranda del cielo).
- **Santuarios:** **3** — R5.0 ("Donde crece" — al cruzar el arco), R5.13.A (en el Arco Oculto), R5.28 ("Donde crece otra vez" — antes del Mirador Final).
- **Luces Cálidas:** 5 — distribuidas por las 5 puertas + 1 en el Arco Oculto.
- **Hazards duros:** **0** — el jardín es completamente seguro. La Raíz no daña.
- **Power-ups disponibles:** Semilla de Luz, Eco de Música, Suspiro Profundo, Escudo de Respiración.

### Ramas opcionales

| ID | Nombre | Acceso | Rooms | Recompensa |
|---|---|---|---|---|
| **N5.A** | El árbol caído (arco oculto) | Bifurcación oculta en R5.13 — pista parallax: una rama caída en el bosque distante | 3 rooms con un mini-laberinto vegetal | Santuario "Donde crece" oculto + Fragmento "Ternura" + 1 Galleta extra |
| **N5.B** | El estanque | Bifurcación visible en R5.17 (sendero lateral con flores) | 1 room interactiva (eco contemplativo) | 2 Páginas finales + −20% peso permanente del nivel |
| **N5.C** | Ventana de Bruno | Bifurcación visible desde el Mirador Final con Semilla de Luz plantada en el borde correcto | 1 room | Cinemática especial: ventana de Bruno se ilumina en el parallax lejano |

### Principio de diseño del nivel

*"Volver a casa, pero cambiado."* N5 es un **nivel de celebración y cierre**. La dificultad mecánica baja ligeramente — ya no se trata de probar habilidad, sino de **usar libremente todo lo aprendido con alegría**. La estructura cambia radicalmente: es el **primer y único nivel con estructura hub-and-spokes** — una plaza central con 5 puertas hacia sub-niveles, que puedes visitar en el **orden que quieras**. Esta es la primera vez en el juego que el jugador tiene **libertad de orden**.

**Twist de diseño:** la Semilla de Luz **modifica el entorno permanentemente**. Es la primera mecánica del juego donde el jugador **crea** mundo en vez de atravesarlo. Narrativamente: *"empiezas a dejar marca propia"*.

**Nota:** si en N4 el jugador eligió "Sí" a La Sombra, esta **lo acompaña todo el nivel** como sprite compañero visible. Su animación es simple pero presente. A veces hace gestos sutiles — se detiene cuando Mateo se detiene, mira en la misma dirección.

### Estructura del nivel

```
                     ┌──────────────────┐
                     │   MIRADOR FINAL   │
                     │   (ascenso libre) │
                     └────────▲─────────┘
                              │
      ┌──────┬──────┬──────┬──┴───┬──────┐
      │      │      │      │      │      │
   PUERTA  PUERTA  PUERTA  PUERTA  PUERTA
    1(7a)   2(10a)  3(12a)  4(14a)  5(16a)
      │      │      │      │      │
      └──────┴──────┼──────┴──────┘
                    │
             ┌──────▼──────┐
             │   PLAZA     │
             │  CENTRAL    │
             └─────────────┘
```

### Fondos (parallax, 7 capas — los más elaborados del juego)

| Capa | Factor | Contenido |
|---|---|---|
| Sky | 0.0 | Cielo de atardecer cálido (naranja-dorado) con nubes |
| Very far | 0.05 | Montañas lejanas con nieve |
| Far | 0.15 | Bosque distante verde-dorado |
| Mid | 0.35 | Árboles grandes + estructuras de piedra antiguas |
| Near | 0.6 | Ramas con hojas en el primer plano de fondo |
| Gameplay | 1.0 | Plataformas de piedra y madera con musgo y flores |
| Foreground | 1.4 | Pétalos volando, luciérnagas |

---

### Plaza Central (rooms 5.0 – 5.2, ~4 min)

**Emoción:** Plenitud serena, promesa. **Enseña:** Estructura hub, libertad de orden.

**R5.0 — "La llegada" — SANTUARIO 1 ("Donde crece")**
Transición desde N4. Mateo (+ Sombra compañera opcional) camina a través de un arco de piedra cubierto de enredaderas. Luz cálida dorada. Música de guitarra completa suena suave. **Primer momento de "paz total" del juego.** Cámara se abre lentamente. Justo bajo el arco, un **árbol joven con linternas de papel colgando** — el primer Santuario del Nivel 5. Heal completo, save real, depósito automático. Texto flotante: *"Llegaste."*

**R5.1 — "La plaza del árbol" (Banca 1)**
Sala gigante circular con un **árbol gigante en el centro** que toca el techo del parallax. Del árbol salen **5 puertas luminosas** a distintas alturas, conectadas por senderos de piedra. **La Chispa** flota cerca de Mateo por primera vez visible en gameplay layer. **El Eco Amable** espera bajo el árbol. Una banca circular rodea las raíces del árbol — el jugador puede sentarse libremente entre puerta y puerta. La Chispa puede ser invocada con [Q] para guiar al jugador hacia la siguiente puerta sugerida (no obligatorio).

**R5.2 — "El Eco Amable explica sin explicar"**
Diálogo corto con el Eco Amable:
*"Este es un lugar donde todo lo que fuiste te espera."*
*"Puedes visitar a cada uno en el orden que quieras."*
*"Ninguno te va a juzgar. Solo quieren verte."*

Entrega la **primera Semilla de Luz**. Prompt de uso: [Q] Plantar Semilla. Donde la plantes, crece una plataforma luminosa permanente. El jugador puede experimentar inmediatamente.

El jugador ahora puede ir a cualquiera de las 5 puertas.

---

### Puerta 1 — "Mateo de 7 años" — LA LUZ (rooms 5.3 – 5.7, ~5 min)

**Tono:** Risa, juego puro, velocidad infantil.

**R5.3 — "Hopscotch de luz"**
Al cruzar la puerta 1, la música cambia a melodía infantil alegre. Plataformas cuadradas que se iluminan en secuencia, como el juego de la rayuela. El jugador debe saltar en secuencia sincronizada con la canción. **Onboarding diegético:** si saltas fuera de ritmo, la plataforma se apaga suavemente — no te hace daño, te indica.

**R5.4 — "Persiguiendo al niño"**
**Mateo de 7 años** aparece corriendo más adelante, riendo. Narrativa: *"¡Alcánzame!"*. El jugador debe seguirlo usando dash + doble salto libremente. Secuencia de ~30 segundos. El niño no se puede perder — siempre reaparece delante.

**R5.5 — "El juego del árbol"**
Sub-zona con un árbol pequeño. El niño trepa con wall jumps. El jugador lo sigue. En la copa, **Semilla de Luz #2**.

**R5.6 — "Tobogán de hojas"**
Plataforma inclinada con hojas brillantes cayendo. El jugador se desliza hacia abajo sin control (mecánica de ice floor). Mateo de 7 años va delante, riendo. Llegan juntos al fondo.

**R5.7 — "Entrega del fragmento + Galleta 1"**
Mateo de 7 años se para, jadeando de felicidad, y entrega el Fragmento **"Alegría Sin Razón"** (+8% velocidad) **+ una Galleta de Memoria Viva** (de un bolsillo lleno de migas). Dice: *"No te olvides de esto. De correr sin razón. Ah — y come algo, mamá hizo galletas."* La Galleta sube el **Aliento máximo +1 permanente**. Desaparece en partículas doradas. Mateo vuelve a la Plaza Central automáticamente.

---

### Puerta 2 — "Mateo de 10 años" — EL RITMO (rooms 5.8 – 5.12, ~5 min)

**Tono:** Curiosidad musical, descubrimiento.

**R5.8 — "El puente musical"**
Plataformas que aparecen sincronizadas a una melodía de piano y guitarra. El jugador debe saltar al ritmo. Si salta fuera de ritmo, cae a una plataforma suave abajo — no pierde, solo repite.

**R5.9 — "La habitación de la guitarra"**
Sala pequeña con una guitarra flotante. **Mateo de 10 años** está sentado tocando. Mira a Mateo presente y sonríe. Le hace un gesto para que se siente.

**R5.10 — "Aprender la canción"**
Mini-puzzle de ritmo. 8 notas deben tocarse en orden (presionando Espacio al ritmo). Si falla, repite. **No es punitivo — es meditativo.** Cada intento suena bonito.

**R5.11 — "La canción completa + Galleta 2"**
Tras completar, la canción sigue sonando en el parallax near-layer. Mateo de 10 años entrega el Fragmento **"Última Guitarra"** (el dash deja un eco sonoro que aturde enemigos) **+ Galleta**. Dice: *"Estabas aprendiendo rápido. No dejes de tocar. Toma — me sobró una de las del recreo."* +1 Aliento máximo permanente.

**R5.12 — "Semilla oculta"**
En el camino de vuelta, si el jugador usa una Semilla de Luz en un punto específico alto, desbloquea una plataforma que lleva a una **Página de Diario escondida** con la letra de la canción. Motivo de rejugabilidad.

---

### Puerta 3 — "Mateo de 12 años" — EL SILENCIO (rooms 5.13 – 5.17, ~5 min)

**Tono:** Contemplativo, nostálgico, tranquilo.

**R5.13 — "Plataformas flotantes en el cielo"**
Sub-zona en el cielo — plataformas suspendidas entre nubes doradas. Sin enemigos, sin prisa. El jugador salta a su ritmo. La música es mínima.

**R5.14 — "El preadolescente en la baranda" (Banca 2)**
**Mateo de 12 años** está sentado en el borde de una plataforma mirando el cielo en silencio. La plataforma cuenta como **Banca**. El jugador se acerca. Mateo se sienta a su lado automáticamente. **30 segundos de silencio real — similar a la banca del N3 pero menos largo.** No hay texto. Solo dos sprites sentados.

**R5.15 — "Las palabras"**
Tras los 30 segundos, Mateo de 12 años habla:
*"A veces pensaba que yo era raro por pensar tanto."*
*"Pero ahora veo que pensar no es malo. Solo cansa si nadie escucha."*
*"Tú me escuchaste. Aunque tarde."*

**R5.16 — "El dibujo en la piedra + Galleta 3"**
Se levanta, camina hacia una piedra grande y la toca. En la piedra aparece un dibujo infantil (un monstruo con alas — algo que Mateo dibujaba). Entrega el Fragmento **"Raíz Cálida"** (+1 vida máxima cuando equipado) **+ Galleta**. Dice: *"Esto era mío. Ahora es tuyo. La galleta también — la tenía guardada para cuando fueras a venir."* +1 Aliento máximo permanente.

**R5.17 — "Camino al jardín"**
Vuelta a la plaza por un sendero lleno de flores brillantes. Las Semillas de Luz se pueden plantar aquí para crear un camino permanente de luz.

---

### Puerta 4 — "Mateo de 14 años" — LA CARRERA (rooms 5.18 – 5.22, ~5 min)

**Tono:** Energía, libertad cinética, camaradería.

**R5.18 — "El corredor infinito"**
Plataformas largas rectas con pocas desniveles. El gameplay invita a usar **dash libre** y doble salto con soltura. Pétalos volando por el parallax foreground. Música cálida y rítmica.

**R5.19 — "Carrera con el yo de 14"**
**Mateo de 14 años** aparece corriendo a la par. No es una carrera por puntos — es solo correr juntos. Si Mateo presente se adelanta, el de 14 le sigue el ritmo. Si se atrasa, lo espera.

**R5.20 — "El salto imposible"**
Al final del corredor hay un hueco enorme — imposible con dash + doble salto normal. Mateo de 14 años se detiene y mira a Mateo presente con una sonrisa. Le dice: *"Vamos. Los dos a la vez."* Ambos saltan. El hueco se cruza. (Mecánicamente: se dispara un salto de fuerza doble que requiere ambos a la vez. Es automático si estás cerca del borde.)

**R5.21 — "La pared"**
Al otro lado, una pared. Ambos Mateos la golpean suavemente. Se rompe en pedazos. Al otro lado, una vista panorámica del jardín entero.

**R5.22 — "Entrega del fragmento + Galleta 4"**
Mateo de 14 años entrega el Fragmento **"Ancla del Silencio"** (−50% chance de debuff) **+ Galleta**. Dice: *"Te enseñé a correr. Acuérdate de eso cuando quieras parar. Y come, que se enfría."* +1 Aliento máximo permanente. Desaparece.

---

### Puerta 5 — "Mateo de 16 años" — LA CONVERSACIÓN (rooms 5.23 – 5.27, ~6 min)

**Tono:** Adulto, reflexivo, difícil, sanador.

**R5.23 — "La habitación oscura"**
Sub-zona intencionalmente con poca luz. Una habitación que se parece al cuarto real de Mateo. **Mateo de 16 años** está sentado en una cama, mirándose las manos. La Chispa flota cerca.

**R5.24 — "La pregunta inversa"**
Mateo presente se sienta frente al de 16. Es la primera vez que **el Mateo presente habla él mismo** (voz narrativa).
Mateo presente: *"¿Te acuerdas de mí?"*
Mateo de 16: *"Claro. Eres yo."*

**R5.25 — "Lo que duele"**
Diálogo largo sin gameplay. Mateo de 16 habla:
*"Cuando Bruno dejó de contestar, pensé que era por mi culpa."*
*"Dejé de tocar porque la guitarra sonaba a él."*
*"No pude decirle a nadie que estaba mal porque pensé que era solo mío."*
*"Pero no era solo mío."*

**R5.26 — "Lo que va a doler"**
Mateo de 16 se levanta, camina hacia Mateo presente y lo abraza. Cinemática única. Dice:
*"A veces te va a doler otra vez. Eso es normal."*
*"Pero ya no vas a estar tan solo. Porque ahora tienes esto."* — señala el inventario.

Entrega el Fragmento **"Voz del Niño"** (dash reset al wall-jump) — o si ya lo tenías desde N4, entrega un fragmento único **"Perdón"** (+2 vidas máximas cuando equipado) **+ Galleta** (+1 Aliento máximo permanente).

**R5.27 — "El camino al mirador"**
Una escalera de luz se forma desde la habitación hacia el exterior. Mateo sube. La habitación se queda vacía pero iluminada.

---

### El Mirador Final (rooms 5.28 – 5.30, ~5 min)

**R5.28 — "Ascenso libre" — SANTUARIO 3 ("Donde crece otra vez")**
Desde la Plaza Central (tras completar las 5 puertas), el árbol gigante despliega ramas como plataformas ascendentes. **Antes** de iniciar el ascenso, en el tronco mismo del árbol, hay una pequeña abertura iluminada — el tercer Santuario del nivel, hermano del primero. **Última oportunidad de save antes del Mirador Final.** Es la única razón mecánica por la que existe: si el jugador eligió "Sí" en la Pregunta Final del N4 y quiere ver el Mirador con la Sombra compañera intacta, debe guardar aquí. El jugador sube libremente usando **TODAS las habilidades aprendidas** + Semillas de Luz para crear atajos. La música de guitarra sube de intensidad.

**R5.29 — "El mirador de la cima"** *(MOMENTO PICO FINAL)*
Plataforma circular en la cima del árbol con vista panorámica completa del parallax: cielo, montañas, bosque, pueblo distante, y muy lejos — un cuarto con una ventana iluminada. El **cuarto de Bruno.**

Al llegar a la cima, las **5 Memorias Vivas** aparecen sentadas en el borde de la plataforma una al lado de la otra. Mateo se sienta entre ellas. **La Sombra compañera** (si está) se sienta al otro lado. Todas las **Galletas comidas** durante el nivel se reflejan visualmente en el sprite de Mateo: su halo de Aliento es ahora el más grande del juego (9 fragmentos si recogió las 5).

Texto final en pantalla: *"No volví solo. Volví con todos los que fui."*

La música de guitarra llega a su clímax. Todas las figuras se vuelven luz y se fusionan con Mateo. Su sprite cambia — ahora su retrato en el menú es rostro completo, ya no silueta.

**R5.30 — "Créditos"**
Fade a blanco. Créditos sobre el parallax del amanecer. Música de guitarra completa.

### Epílogos (depende de elecciones)

Tras los créditos:

**Si elegiste RESPONDER el teléfono en N1 y clasificaste al menos 7 objetos como ENTENDER en N2:**
Mensaje en pantalla negra: *"Respondiste. Bruno está al otro lado. Habla con él."* → Abre cinemática alternativa de 30 segundos donde Mateo marca el teléfono de Bruno (sonido de ring). Bruno contesta: *"Hola."* Silencio. Mateo dice: *"Hola, Bruno."* Fade.

**Si encontraste la Llave Oxidada en N3:**
Aparece un mensaje: *"Hay una banca esperando. ¿Quieres visitarla?"* — **Abre el Nivel Secreto.**

**Si completaste todo al 100%:**
Los dos epílogos anteriores se combinan y añade una cinemática final de Mateo, Bruno, la Sombra y las Memorias Vivas sentados juntos mirando el atardecer.

**Mensaje final obligatorio (todos los finales):**
Texto en pantalla sobre negro:
*"Si tú o alguien que conoces está luchando:*
*Línea de Crisis [número real]*
*No estás solo. Pedir ayuda es fuerza."*

---

### Secretos del Nivel 5

Etiqueta: 🌿 = Rama opcional / 🔍 = Pista en parallax / ⏳ = Backtrack futuro / 💫 = Acción específica

| # | Ubicación | Categoría | Contenido |
|---|---|---|---|
| S1 | R5.12 — techo de la habitación de guitarra | 💫 | Página de Diario con la letra de la canción |
| S2 | R5.13 — Rama N5.A "El árbol caído" (pista parallax) | 🌿🔍 | **Santuario "Donde crece" oculto** + Fragmento **"Ternura"** + 1 Galleta extra (sube max +1) |
| S3 | R5.17 — Rama N5.B "El estanque" | 🌿 | 2 Páginas finales + −20% peso permanente |
| S4 | R5.17 — plantar Semillas en 3 puntos específicos | 💫 | Fragmento alternativo **"Sendero Iluminado"** |
| S5 | R5.23 — esquina oscura de la habitación | 💫 | Frase Oculta: *"El silencio también era amor."* |
| S6 | R5.29 — Rama N5.C "Ventana de Bruno" (Semilla en borde correcto) | 🌿💫 | Ilumina una ventana lejana en el parallax (Bruno en su cuarto) |

### Diálogos clave

- R5.2: *"Puedes visitar a cada uno en el orden que quieras. Ninguno te va a juzgar."* (Eco Amable)
- R5.7: *"No te olvides de esto. De correr sin razón."*
- R5.11: *"No dejes de tocar."*
- R5.15: *"Pensar no es malo. Solo cansa si nadie escucha."*
- R5.20: *"Vamos. Los dos a la vez."*
- R5.26: *"A veces te va a doler otra vez. Eso es normal."*
- R5.29: *"No volví solo. Volví con todos los que fui."*

---

## Nivel Secreto — LA BANCA COMPARTIDA

| Campo | Detalle |
|---|---|
| **Requisito de acceso** | Llave Oxidada (Nivel 3, Rama N3.A Fondo Luminoso con peso < 50%) |
| **Duración** | 10-12 minutos |
| **Enemigos** | Ninguno |
| **NPCs** | Bruno |
| **Habilidades usadas** | Ninguna — el dash y el salto están **deshabilitados**. Solo caminar. |

### Sistemas activos en el nivel

- **Enfoque:** **deshabilitado.** No hay chispas. No hay recurso de movimiento. Mateo solo camina.
- **Aliento:** **inmortal.** El sistema de daño está apagado. Mateo no puede perder fragmentos. Visualmente el halo está al máximo en oro absoluto.
- **Peso Emocional:** **deshabilitado.** No existe en este nivel.
- **Bancas:** ninguna (no son necesarias).
- **Santuarios:** ninguno (el nivel mismo es un santuario lineal).
- **Luces Cálidas:** ninguna (no hay nada que recargar).
- **Hazards duros:** ninguno.
- **Power-ups:** ninguno.
- **Save:** **automático al inicio del nivel** y **automático al final** — el progreso del nivel secreto se guarda como flag en el save permanente, no como save de sub-room. Si cierras el juego durante el nivel secreto, vuelves al inicio del nivel secreto al cargar.
- **Fail state:** **inexistente.** No puedes Desvanecerte en este nivel. La única forma de "salir" es completarlo o cerrar el juego.

**Por qué todo está apagado:** ver la nota de diseño al final del nivel — el nivel secreto es la antítesis mecánica del juego. Todas las herramientas que el jugador acumuló se vuelven irrelevantes para forzar la emoción de "estar y nada más".

### Principio de diseño del nivel

*"Habla con alguien. Siempre hay alguien al otro lado."* El nivel secreto es un **walking simulator intencional**. No hay habilidades mecánicas. No hay peligro. **No hay dash, ni salto, ni power-ups.** Solo caminar y escuchar. Es la **antítesis mecánica** del resto del juego — un nivel donde el único verbo es **presencia**.

La razón del diseño: el jugador, después de 2+ horas de platforming avanzado, se ve reducido a un solo verbo. Esto **fuerza mecánicamente** la emoción que el nivel quiere provocar: no hay nada que hacer excepto estar ahí. El platformer como ruido mental queda callado.

**El nivel dura lo que dura la conversación.** El jugador no puede acelerarla. No puede saltarla. No hay botón de skip en los diálogos.

### Fondos (parallax, 4 capas — paleta cálida de atardecer)

| Capa | Factor | Contenido |
|---|---|---|
| Sky | 0.0 | Atardecer naranja-rosa con nubes doradas |
| Far | 0.1 | Montañas muy lejanas, siluetas de la ciudad de la infancia |
| Mid | 0.3 | Árboles del parque, siluetas de casas |
| Gameplay | 1.0 | Sendero de piedra con bordes de hierba |

---

### R-Secreto.1 — "La puerta oxidada"

Aparece tras los créditos del Nivel 5 (solo si tienes la Llave Oxidada). Mensaje en pantalla: *"Hay una banca esperando. ¿Quieres visitarla?"* con opción **[E] Visitar**. Si elige no, el juego termina normal y la opción queda disponible desde el menú principal.

Si elige sí: Mateo camina hacia una puerta de madera oxidada. La abre con la llave. Fade a atardecer.

### R-Secreto.2 — "El sendero"

Mateo aparece en un sendero de parque al atardecer. **No hay HUD.** Solo el mundo. El único control disponible es A/D (caminar hacia la derecha). Espacio no hace nada. Shift no hace nada. Intentar saltar muestra un pequeño texto flotante: *"No hay prisa."*

El sendero es largo. ~2 minutos de caminata sin eventos. En el parallax:
- Niños jugando al fondo (muy lejos, siluetas)
- Un perro corriendo
- Una pareja sentada bajo un árbol
- Hojas cayendo

Un Mateo del pasado (12 años) aparece fugazmente en el parallax mid-layer, jugando. No se detiene.

### R-Secreto.3 — "Caminar más"

El sendero continúa. Al minuto 3, el jugador ve a lo lejos una **banca**. Sentado en la banca, una figura. Mateo camina más despacio automáticamente (velocidad reducida al 80%).

### R-Secreto.4 — "Reconocimiento"

A 10 tiles de la banca, Mateo se detiene automáticamente. Cámara hace close-up en la cara del otro chico. Es **Bruno**. Está mirando hacia el suelo, con los audífonos puestos.

Texto flotante (narrativo, sin quién lo dice): *"Pensé que no iba a reconocerlo. Pensé que me iba a dar miedo."*

El jugador debe seguir caminando. Los últimos 10 tiles se sienten largos.

### R-Secreto.5 — "Sentarse"

Al llegar a la banca, Mateo se sienta automáticamente. No hay input. Cámara hace pan al otro lado de la banca mostrando a los dos juntos. **30 segundos de silencio real.** Bruno no lo ha notado. Mateo lo mira.

A los 30 segundos, Bruno se quita un audífono.

### R-Secreto.6 — "La conversación"

Diálogo extenso, **automático**, sin opciones. El jugador solo lee. Cada frase aparece con tiempo para leerla (2-4 segundos por frase).

**Bruno:** *"Ey."*
**Mateo:** *"Ey."*
(Pausa 3s.)
**Bruno:** *"¿Hace cuánto no nos veíamos?"*
**Mateo:** *"No sé. Mucho."*
(Pausa 4s.)
**Bruno:** *"Creí que te había perdido."*
(Pausa 2s.)
**Mateo:** *"Yo también creí que te había perdido."*
(Pausa 5s.)
**Bruno:** *"Yo también estuve mal. No sabía cómo decirlo."*
**Mateo:** *"Yo tampoco."*
(Pausa.)
**Bruno:** *"Perdón por no contestar."*
**Mateo:** *"Perdón por no preguntar."*
(Pausa larga 8s.)

### R-Secreto.7 — "El flashback"

Durante la pausa larga, el parallax entero se disuelve por 10 segundos en un **flashback visual** — los dos chicos de niños, con guitarras, tocando juntos en un cuarto. La canción incompleta del Nivel 2 suena **completa por primera vez**. La melodía es hermosa y simple.

Al terminar el flashback, vuelven a la banca al atardecer. Nada ha cambiado visualmente. Pero algo cambió.

### R-Secreto.8 — "El silencio compartido"

**Bruno:** *"¿Quieres quedarte un rato?"*
**Mateo:** *"Sí."*

Texto en pantalla, sin quién lo dice: *"El silencio también puede ser amor, si lo compartes."*

**60 segundos de silencio real.** Los dos sprites sentados. La música de guitarra completa sigue sonando muy suave. El sol baja lentamente en el parallax. Las luciérnagas aparecen una a una.

### R-Secreto.9 — "El cassette"

Si el jugador encontró el **Cassette oculto en el Nivel 2**, aparece una mini-interacción única aquí: Bruno saca un cassette player viejo del bolsillo.
**Bruno:** *"Grabé algo para ti hace mucho. Nunca te lo di."*
Inserta el cassette. Suena una canción adicional (30 segundos) que solo los completionistas escuchan.

### R-Secreto.10 — "El cierre"

Fade a negro muy lento (15 segundos). La música permanece. Sobre el negro, aparece el texto final:

> *"Habla con alguien."*
>
> *"Siempre hay alguien al otro lado."*
>
> *"Si tú o alguien que conoces está luchando, no estás solo."*
>
> *"Busca a un adulto de confianza, un amigo, un profesional."*
>
> *"Pedir ayuda no es rendirse. Es el acto más valiente que puedes hacer."*

**Línea de Crisis Psicológica (Perú):**
*Línea 113 (MINSA) — Opción 5*
*Disponible 24/7, gratuita, confidencial.*

*El juego se cierra automáticamente tras 30 segundos. No hay botón de continuar.*

---

### Nota de diseño del equipo

El nivel secreto **no es un desafío**. Es una recompensa. No por dominar el juego, sino por **cuidarlo**. El jugador encuentra la Llave Oxidada solo si llegó al Fondo Luminoso del Nivel 3 — es decir, solo si cuidó a Mateo lo suficiente para usar las Anclas Sensoriales y bajar su peso emocional. El juego premia la ternura, no la habilidad.

Esta es la declaración de intenciones final del proyecto: **pedir ayuda salva vidas. Estar con alguien en silencio también.**

---

# V. ARTE Y AUDIO

## 15. Estilo visual

- **Pixel art moderno 2D** con resolución de referencia ~320x180 escalada a 1920x1080.
- **Paleta limitada** por nivel para reforzar la emoción:
  - N1: grises industriales + azules cansados + destellos amarillos de ansiedad
  - N2: negros profundos + dorados de memoria
  - N3: grises → azules profundos → púrpuras de madrugada
  - N4: rojos oscuros + blancos quebrados
  - N5: verdes cálidos + naranjas de atardecer + dorados
- **Fondos 2.5D:** cada nivel usa 4-6 capas de parallax. Las capas lejanas son siluetas casi monocromáticas; las cercanas ganan detalle.
- **Iluminación 2D (URP Light2D):** luces cálidas en zonas seguras, luces frías/pulsantes en zonas peligrosas, linterna sutil en Mateo durante zonas oscuras.
- **Post-processing dinámico por nivel:**
  - **N1:** viñeta pulsante leve en zonas de alarma; chromatic aberration sutil al recibir Tunnel Vision.
  - **N2:** desaturación parcial cuando los controles están invertidos (señal visual + de input).
  - **N3:** desaturación global gradual con el peso emocional; recupera color tras la Banca de 60s.
  - **N4:** filtro espejado en algunas rooms (eje vertical) — la cámara muestra el reflejo en lugar del mundo durante 2-3s en momentos clave.
  - **N5:** bloom suave cálido + lens flare ocasional al amanecer.

## 15.b Lenguaje visual de estado (nuevo en v2.1)

Esta sub-sección codifica las reglas de feedback visual del refinamiento v2.1. Es la fuente única para cómo se muestra cada estado del jugador, cómo se comunica daño, curación, save, y costo, y qué colores significan qué cosa en todo el juego.

### HUD diegético (no UI tradicional)

El HUD oficial muestra **únicamente**:
- Contador de **Destellos seguros** (esquina superior izquierda, dorado fijo, font pequeña).
- Contador de **Destellos en bolsa** (debajo, dorado parpadeante).
- **Prompt de interacción** contextual (centro inferior, solo cuando hay un IInteractable cerca).
- **Cantidad de Tés de Tilo** disponibles (esquina inferior izquierda, ícono de taza con número).

**No hay barra de HP, no hay barra de stamina, no hay barra de power-up con timer en pantalla.** Todo eso vive en el sprite de Mateo:

| Estado | Cómo se muestra |
|---|---|
| **Aliento** | Halo pulsante alrededor del sprite con N "respiraciones" (1 pulso por fragmento). A 1 fragmento: tono rojo apagado y pulso rápido. |
| **Enfoque** | 2-5 chispas blancas que orbitan suavemente el sprite. Se apagan al gastarse, se encienden al recargar. Vacío: 1 chispa parpadea tenue. |
| **Power-up activo** | Aura del color del power-up + ícono pequeño flotando sobre la cabeza con un círculo de tiempo restante. |
| **Debuff activo** | Efecto fullscreen específico (viñeta para Tunnel Vision, inversión de cámara para Controles Invertidos, blanco súbito para Flash de Recuerdo). |
| **Peso Emocional** | Niebla gris-azul sutil envolviendo el sprite, más densa a más peso. En N3 además barra UI vertical opcional. |
| **Sin Enfoque (Exhausted)** | Mateo cambia a animación `Exhausted`: postura encorvada, idle más lento, respiración audible. |
| **A 1 fragmento de Aliento** | Latido cardíaco audible bajo en el SFX + halo rojo pulsante. |

### Lenguaje de color universal (semántico, no estético)

Esta paleta semántica se usa en TODOS los niveles aunque su LUT principal cambie.

| Color | Significado |
|---|---|
| **Dorado cálido** | Seguro, vida, save, narrativa positiva, Destellos, Santuarios, Bancas activas |
| **Cyan suave** | Descanso, calma, breath, agua narrativa, Latido Calmado |
| **Rojo apagado** | Daño, hostilidad, presión, hazards estándar |
| **Rojo brillante** | **Hazard duro** (Desvanecerse instantáneo). Siempre visible y advertido. |
| **Morado / púrpura** | Ilusión, voces, distorsión, Las Voces |
| **Blanco puro** | Flash de recuerdo, momento de revelación, Desvanecerse |
| **Verde oliva** | Integración, cierre, jardín, transición a N5 |
| **Gris monocromo** | Depresión, peso, vacío, fondo del N3 |
| **Naranja-dorado** | Eco Amable, Memoria positiva, calidez familiar |

### Reglas de feedback inmediato

Cada acción del jugador tiene **3 capas de feedback obligatorias**: visual (sprite/partícula), audio (SFX) y haptic (screen shake o pulse de cámara). Una acción sin las 3 capas es un bug de diseño.

**Reglas específicas:**
- **Daño recibido:** 0.3s freeze frame + zoom súbito + flash rojo en el sprite (no fullscreen) + screen shake suave + SFX bajo.
- **Heal recibido:** partículas doradas espirales al sprite + tono cálido en el halo + suspiro audible.
- **Recoger Destello:** partículas pequeñas + ding agudo + flash dorado en HUD bolsa.
- **Recoger coleccionable narrativo:** partículas + SFX único por tipo + texto flotante de 2-3s con frase asociada.
- **Activar Banca:** fade amarillo lento + chime + ligera vibración de cámara.
- **Activar Santuario:** secuencia ritual de 1.5s SI O SÍ — animación `Sanctuary`, motif de guitarra (3 notas), partículas doradas espirales, flash blanco breve, fade-back a gameplay con HUD restaurado.
- **Desvanecerse:** ver §13.c para la secuencia completa.
- **Recoger Luz Cálida:** la brasa "exhala" sus partículas hacia el sprite, halo de Aliento se llena visiblemente, chispas de Enfoque se encienden una por una con un tono cada una.

### Pista visual del parallax como gameplay

Refinamiento v2.1: el parallax no solo decora — es una **pista de exploración válida**. Cada nivel tiene 1-2 secretos cuya única señal está en una capa de fondo (ver tabla de Secretos por nivel, etiqueta 🔍). Esto enseña al jugador que **mirar al fondo es jugar también**.

## 16. Animación de Mateo

| Estado | Frames | Particularidad |
|---|---|---|
| Idle | 4 | Respiración visible |
| Run | 8 | Con rastro de partículas si tiene Fragmento equipado |
| Jump | 3 | Anticipación, subida, caída |
| Fall | 2 | — |
| Wall slide | 2 | Rastro de polvo |
| Dash | 3 | Motion blur horizontal |
| Crouch | 2 | — |
| Hit | 2 | Flash rojo |
| Sit (banca) | 1 | Usado en momentos clave (N3, N5, secreto) |
| **Heal** ★ | 3 | Mateo se acuclilla y bebe Té de Tilo (~1.5s no cancelable) |
| **RestSit** ★ | 2 | Sentarse en Banca con respiración profunda |
| **Sanctuary** ★ | 4 | Ritual de respirar profundo en Santuario (1.5s) |
| **Exhausted** ★ | 2 | Idle alternativo cuando Enfoque está vacío — postura encorvada |
| **Crisis** ★ | 3 | Animación de Desvanecerse: caer de rodillas + cerrar los ojos |

★ Animaciones nuevas en v2.1, asociadas a los nuevos sistemas.

## 17. Música y sonido

- **Instrumento principal:** guitarra acústica (tema de Mateo) — incompleta durante N1-N4, completa al final.
- **N1:** percusión irregular + zumbidos eléctricos industriales.
- **N2:** piano lento con notas faltantes + celesta cuando clasificas "ENTENDER".
- **N3:** silencio casi total. Solo niebla ambiental. Cada Ancla Sensorial añade una nota de piano que persiste.
- **N4:** cuerdas tensas + cristal quebrándose.
- **N5:** guitarra completa + coro sutil.
- **SFX importantes:** paso doble (Mateo), salto suave, dash wushh, puñado de hojas en cada recolección, latido cardíaco durante los debuffs de ansiedad.

### Motifs de sistema (nuevos en v2.1)

Cada sistema central tiene su propio breve motif sonoro reconocible:

- **Santuario:** 3 notas de guitarra ascendentes (Mi-Sol-Si) al activarse + colchón armónico cálido durante el ritual de 1.5s. Cada nivel tiene una variación leve del mismo motif (mismo intervalo, distinta resonancia).
- **Banca:** una sola nota de piano + silbido leve de viento.
- **Desvanecerse:** **fade absoluto** del audio en 0.4s (sub-bass se mantiene). 1s de silencio antes del texto. Una sola nota grave de guitarra al respawn en el Santuario.
- **Recoger Té de Tilo / heal:** suspiro audible + tono grave-cálido que sube ½ tono.
- **Luz Cálida:** SFX de inhalación humana suave + brasa apagándose como ronquido tenue.
- **Recargar chispa de Enfoque:** tic muy bajo, casi inaudible. La chispa al gastarse es la mitad del volumen — la inhalación al recargar también — para que ninguna se vuelva irritante con repetición.
- **Stomp exitoso:** plop con eco corto + un mini-chime de Destello/Enfoque ganado.

---

# VI. IMPLEMENTACIÓN TÉCNICA

## 18. Stack

- **Motor:** Unity 6 LTS, URP (Universal Render Pipeline 2D)
- **Plataformas objetivo:** PC primero, WebGL segundo
- **Física:** Rigidbody2D + BoxCollider2D/CircleCollider2D + Physics2D.OverlapBox para checks
- **Input:** Input System de Unity (ya configurado en `InputSystem_Actions.inputactions`)
- **Tiles:** Unity Tilemap 2D ortogonal con CompositeCollider2D
- **Iluminación:** Light2D (URP 2D Renderer)

## 19. Arquitectura de scripts

Detallada en [PROGRESO.md](PROGRESO.md). Namespace root: `Shatter`. Sub-namespaces por dominio. Los scripts marcados ★ son nuevos en v2.1 (Sprint 1 planeado):

```
Shatter.Core          → GameManager, ObjectPool, AudioManager, GameEvents
Shatter.Player        → PlayerController2D, PlayerHealth, IdentityManager,
                        ★ FocusSystem, ★ EmotionalWeight
Shatter.Systems       → PowerUpManager, DebuffManager, Collectible,
                        ★ Bench (renombrado de Checkpoint),
                        ★ SanctuarySystem, ★ WarmLight, ★ TeaItem
Shatter.CameraSystem  → CameraFollow2D, ParallaxLayer
Shatter.AI            → EnemyBase, WalkerEnemy, FlyerEnemy
Shatter.Levels        → LevelManager, Level01Manager, OneWayPlatform, Hazard
Shatter.UI            → HUDManager (refactor a HUD diegético), PauseMenu
Shatter.Utils         → Placeholders, SceneBootstrapper
Shatter.ScriptableObjects → IdentityFragmentSO, PowerUpSO, DebuffSO,
                            ★ SanctuaryDataSO, ★ ConsumableSO
```

## 20. Patrones de diseño

- **Singleton:** `GameManager`, `ObjectPool`, `AudioManager`
- **Object Pool:** power-ups, partículas, proyectiles conceptuales, floating text, coleccionables, Tés de Tilo
- **State Machine:** FSM ligera en enemigos (Patrol → Detect → Chase → Return)
- **Observer:** `GameEvents` estático como bus de eventos — desacopla UI, VFX, AI, progreso, sistemas de Aliento/Enfoque/Peso
- **ScriptableObject:** datos de PowerUp, Debuff, IdentityFragment, Enemy stats, **`SanctuaryDataSO`** (visual + nombre + nivel) y **`ConsumableSO`** (efecto + animación + stack máx)
- **Command/Interface:** `IInteractable`, `IPoolable`, `IHealable` (nuevo — implementado por `PlayerHealth` y consumido por consumibles + power-ups)
- **Strategy:** **`FocusSystem`** acepta `IFocusModifier` por escena para aplicar reglas de recarga distintas en cada nivel sin código por nivel

---

# VII. REQUISITOS ACADÉMICOS UPN

| Requisito | Cumplimiento |
|---|---|
| **Patrón Singleton** | `GameManager`, `ObjectPool`, `AudioManager` |
| **Object Pooling** | Sistema completo en `ObjectPool.cs` + `IPoolable` |
| **Personalización del PJ** | Fragmentos de Identidad (`IdentityFragmentSO` + `IdentityManager`) afectan sprite, stats y partículas |
| **ScriptableObjects** | `IdentityFragmentSO`, `PowerUpSO`, `DebuffSO` |
| **Sistema de FSM** | Enemigos con estados (Patrol/Detect/Chase) |
| **Tilemap** | Nivel 1 usa `Grid + Tilemap + CompositeCollider2D` |
| **Input System nuevo** | `InputSystem_Actions.inputactions` presente; por migrar desde input legacy del prototipo |
| **Escena de prototipo jugable** | `SampleScene` + `SceneBootstrapper` |
| **Responsabilidad social** | Tema de salud mental adolescente + mensaje final con recursos reales de apoyo psicológico |

---

# VIII. LÍNEAS CLAVE DEL JUEGO

- *"No sé si lo que siento tiene nombre. Pero sé que lo siento."*
- *"El mapa no me dice dónde estoy. Solo dónde podría ir."*
- *"La sombra no era mi enemiga. Era yo, intentando sobrevivir."*
- *"Hay días en que florecer es solo no rendirse."*
- *"Todavía no sé tocar bien. Pero voy a aprender de nuevo."*
- *"Descansar también es avanzar."*
- *"Cuando me falta el aire, también me faltan los pasos."* — sobre el Enfoque
- *"Guardar lo que vale es respirar profundo."* — sobre los Santuarios
- *"Está bien tomarlo. No es trampa."* — La Sombra, sobre las Luces Cálidas tras 3 muertes
- *"Habla con alguien. Siempre hay alguien al otro lado."* — epílogo

---

**Última actualización del documento:** 2026-04-11 — Refinamiento v2.1: sistemas de Aliento, Enfoque, Save de dos tiers, fail state "Desvanecerse", Destellos como moneda, Luces Cálidas, ramas de exploración, lenguaje visual de estado, reconciliación de contradicciones internas, refinamiento de cada nivel.

**Estado técnico actual:** Core + Nivel 1 prototipo en construcción (Sprint 0). La implementación de los sistemas v2.1 está planeada para el Sprint 1. Ver [PROGRESO.md](PROGRESO.md).
