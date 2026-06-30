# INWARD — Documento de Diseño

> **Versión 3.0 — Linearización (2026-06-04)**
>
> Esta es la versión vigente del documento. La v3.0 **linealiza y simplifica** el diseño de niveles, pasando de ~152 rooms con 11 ramas opcionales a **74 rooms estrictamente lineales** (sin bifurcaciones). Los sistemas, personajes, arcos emocionales y momentos pico narrativos se preservan intactos — solo cambia la arquitectura espacial y la complejidad de implementación. La referencia arquitectónica detallada de cada nivel (dimensiones, diagramas ASCII, blueprints de anclas y clímax) ahora vive en [`niveles_plano.md`](niveles_plano.md); este documento conserva la **definición de sistemas, mecánicas, personajes, narrativa y guía de implementación**.
>
> **Cambios principales v3.0 (vs v2.1):**
> - N1: 31 → 14 rooms · N2: 28 → 14 · N3: 26+4 → 14 · N4: 26 → 14 · N5: 31 → 8 (lineal). Secreto: 10 (sin cambios).
> - N3: estructura vertical simplificada (descenso + banca 60s + ascenso, 2 tramos claros).
> - N4: 3 "Preguntas de La Sombra" consolidadas en 1 sola (R4.9).
> - N5: hub-and-spokes eliminado, 5 memorias ambientales en secuencia.
> - **Llave Oxidada** movida de N3 (Rama eliminada) al Nivel Secreto R-S.1.
> - **Aliento max** unificado a **9** fragmentos (4 base + 5 Galletas de Memoria en N5).
> - Inconsistencias de §8.1 resueltas: Aliento max, Acero Mental fuera del flujo principal, 5 fragmentos narrativos consolidados.
>
> Documentos relacionados: [`niveles_plano.md`](niveles_plano.md) (arquitectura de rooms, blueprints, coordenadas) · [`PROGRESO.md`](PROGRESO.md) (estado de implementación).

---

# I. INTRODUCCIÓN Y CONCEPTO

## 1. Información general

| Campo | Detalle |
|---|---|
| **Nombre** | INWARD |
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

INWARD es la historia de **Mateo**, un adolescente de 17 años que un día despierta y descubre que no puede salir de su propia mente. El mundo exterior sigue girando, pero él está atrapado adentro — en un universo interior hecho de miedos, recuerdos y pequeñas luces que todavía no se apagan. Para volver a casa debe atravesar cinco mundos que representan estados emocionales, enfrentar a una sombra que es él mismo, y aprender que **pedir ayuda no es rendirse**.

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

### Las Memorias Ambientales (Nivel 5)
Recuerdos vivos de Mateo a distintas edades (7, 10, 12, 14, 16). No aparecen como NPCs físicos ni requieren sprites de Mateo joven: cada memoria se manifiesta como un objeto, una luz, un sonido o una pequeña escena del jardín. El **Eco Amable** guía la lectura emocional de cada sala y entrega el **Fragmento de Identidad** + una **Galleta** (que sube +1 Aliento máximo permanente). La Sombra, si fue aceptada en N4, acompaña como presencia silenciosa y reconciliada. Detalles de estructura y diálogos en §IV Nivel 5.

### Bruno (Nivel secreto)
El mejor amigo. Solo aparece si el jugador tiene la **Llave Oxidada** (entregada al inicio del propio Nivel Secreto en R-S.1 — v3.0: ya no se encuentra en el Nivel 3, eso cambió con la linearización). Sentado en una banca al final de un nivel lineal, solo. Mateo se sienta a su lado. No hay combate. Solo escuchar. Desbloquea un epílogo.

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
- **Máximo:** **9 fragmentos** absolutos (4 base + 5 Galletas de Memoria entregadas una por cada memoria ambiental en N5: R5.2, R5.3, R5.4, R5.5, R5.6). Cada Galleta sube el máximo +1 de forma permanente.
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
  6. **Galleta de Memoria** (N5) — sube el máximo de Aliento permanentemente +1. Se obtiene una por cada memoria ambiental en R5.2, R5.3, R5.4, R5.5 y R5.6 (5 totales → max absoluto 9).
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
| 91–100% | (Solo posible en N3 sin Anclas) Sprite arrastra los pies, , doble salto bloqueado. **NO mata.** Es la metáfora de "no puedes moverte". La única salida es caminar muy despacio hasta una Ancla. |

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
  - Drop raro de memorias ambientales en N5.
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
|  (N5) | Sube max +1 perm | — | 5 únicas en N5 | — |

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
| Mini-interacción con una memoria ambiental (N5) | +3 |
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

**Coleccionar ≠ Equipar:** puedes tener hasta 9 fragmentos en tu inventario pero solo 3 activos a la vez. Cambiar es libre desde un Santuario, también desde pausa pero con animación de 1.5s "respirando" para evitar swap táctico instantáneo en pleno combate.

> **Cambio v3.1:** el set se organiza en dos categorías claras: los **5 Fragmentos Narrativos** (los que dan el estado cosmético "Integración" y se entregan uno por nivel como ancla narrativa) y los **Fragmentos de Memoria Ambiental** (los 4 cosméticos extra de N5 que no afectan la Integración). Esto resuelve la inconsistencia previa de v2.1 que mezclaba ambos sets y evita depender de sprites de Mateo joven.

### Los 5 Fragmentos Narrativos (anclas narrativas, dan "Integración")

Estos son los que se desbloquean en momentos emocionales clave de la historia, uno por nivel. Al tener **los 5 en el inventario** — equipados o no — el retrato de Mateo en el menú principal y en el inventario cambia de silueta a **rostro completo** (estado cosmético "**Integración**"). No otorgan stats extra — son el reconocimiento visual de que recuperaste lo que importa.

| # | Fragmento | Nivel / Room | Visual | Efecto mecánico |
|---|---|---|---|---|
| 1 | **Aura de la Púa** | N1 — R1.7 | Tint naranja cálido + partículas de polvo dorado | +5% velocidad |
| 2 | **Tinta de Estrellas** | N2 — R2.12 | Tint azul celeste + rastro de luces estelares | +1 salto extra (wall jump) |
| 3 | **Chispa de Atreverse** | N3 — R3.8 | Aura blanca pulsante + chispa central | Desbloquea **dash** (no es stat, es la habilidad en sí) |
| 4 | **Voz del Niño** | N4 — R4.7 | Bubble de luz infantil | Resetea el dash al hacer wall-jump |
| 5 | **Última Guitarra** | N5 — R5.3 (memoria ambiental "El Ritmo") | Notas musicales flotantes | El dash deja un eco sonoro que aturde enemigos en línea recta |

### Fragmentos de Memoria Ambiental (cosméticos extra de N5, no afectan "Integración")

Estos 4 se entregan junto a las 5 Galletas en las salas de memoria ambiental de N5. Son flavor y stats puros — no son parte del set de los 5 narrativos. Cada uno viene con su Galleta que sube +1 Aliento máximo.

| # | Fragmento | Sala N5 | Visual | Efecto |
|---|---|---|---|---|
| 1 | **Alegría Sin Razón** | R5.2 (La Luz, 7 años) | Destellos multicolores saltarines | +8% velocidad |
| 2 | **Raíz Cálida** | R5.4 (El Silencio, 12 años) | Partículas rojo-naranja | +1 fragmento de Aliento máximo (efecto visual; el +1 real viene de la Galleta) |
| 3 | **Ancla del Silencio** | R5.5 (La Carrera, 14 años) | Halo azul tenue | −50% chance de recibir debuff |
| 4 | **Perdón** | R5.6 (La Conversación, 16 años, **solo si ya tienes Voz del Niño de N4**) | Luz cálida envolvente | +2 fragmentos de Aliento máximo (efecto visual; el +1 real viene de la Galleta) |

> **Nota:** R5.6 entrega **Voz del Niño** si aún no la tenías (caso de replays o ruta sin N4.7); si ya la tienes, entrega **Perdón** como variante. Esta entrega ocurre a través del Eco Amable y una reacción silenciosa de La Sombra, no mediante un sprite de Mateo joven. **Acero Mental** (v2.1) fue removido del flujo principal en v3.0 — se reserva para expansiones futuras o secretos opcionales.

### Narrativa del sistema

Los Fragmentos representan **partes de Mateo que se rompieron al alejarse Bruno**. Recuperarlos es literalmente *volver a ser él*. El juego lo indica visualmente: al tener los 5 Fragmentos Narrativos, el retrato cosmético cambia. Las Galletas y los Fragmentos de Memoria Ambiental, en cambio, son el regalo específico de N5 — representan el **alimento emocional** que cada recuerdo le ofrece al presente. No son obligatorios para la Integración, pero dan +5 Aliento máximo entre todos.

### Habilidades progresivas — desbloqueo diegético (geométrico vs narrativo)

**Mateo no empieza con todas sus habilidades.** Al comienzo del juego solo puede **caminar y saltar**. Las nuevas habilidades vienen por dos vías diegéticas distintas:

- **Diegético geométrico:** la arquitectura del nivel obliga a usar la habilidad para avanzar. Sin fragmento, sin cinemática, sin texto. Crouch, drop-through y wall slide son así — el techo baja, la plataforma es one-way, la pared es muy alta. El jugador *descubre* la mecánica por tentativa.
- **Diegético narrativo:** la habilidad se desbloquea al recoger un **Fragmento de Identidad ligado a un recuerdo emocional**. Hay micro-cinemática (3s), texto flotante con el recuerdo, y una "primera sala test" inmediata. Doble salto, wall jump, dash, dash-reset y eco-sónico son así.

No hay menús de "habilidades desbloqueables" ni tutoriales verbales — el jugador siente que *Mateo recordó cómo se hacía*.

| Habilidad | Nivel | Tipo | Fragmento asociado | Recuerdo que representa |
|---|---|---|---|---|
| Caminar + Saltar | — | default | (default) | Moverse en el mundo |
| **Doble salto** | N1 (R1.7) | narrativo | Aura de la Púa | "Recuerdo estar emocionado por algo" |
| **Crouch + Drop-through** | N1 (R1.10, R1.11) | geométrico | — | Pasar por debajo de las cosas |
| **Wall slide** | N2 (R2.8) | geométrico | — | Sostenerte cuando te falla el suelo |
| **Wall jump** | N2 (R2.12) | narrativo | Tinta de Estrellas | "Recuerdo haber escrito un cuento sobre estrellas con nombre" |
| **Dash** | N3 (R3.8) | narrativo | Chispa de Atreverse | "Recuerdo haber saltado de un muro sin mirar" |
| **Dash reset en wall-jump** | N4 (R4.7) | narrativo | Voz del Niño | "Recuerdo que me tiraba de árboles sin pensar" |
| **Eco sónico del dash** | N5 (R5.3) | narrativo | Última Guitarra | "Recuerdo la primera canción que escribí" |

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
| **Tés de Tilo** ★ | Todos | ~10 (uno por Banca de primer toque + drops) | Heal +1 fragmento (§11.b) |
| **Luces Cálidas** ★ | Todos | 17 totales (3-5 por nivel) | Recarga total Enfoque + heal +1 + −20% peso (§11.b) |
| Ecos del Pasado | Todos | ~20 (Unificado) | Lore narrativo (texto flotante simple) |
| **Fragmentos de Identidad — 5 narrativos** | 1-5 | 5 (1 por nivel: Aura Púa, Tinta Estrellas, Chispa Atreverse, Voz Niño, Última Guitarra) | Dan estado cosmético "Integración" al tener los 5 (§13) |
| **Fragmentos de Identidad — 4 Memoria Ambiental** | 5 | 4 (R5.2 Alegría, R5.4 Raíz, R5.5 Ancla, R5.6 Perdón condicional) | Flavor + stats puros (no Integración) |
| **Galletas de Memoria** | 5 | 5 (una por cada memoria ambiental, R5.2 a R5.6) | +1 Aliento máximo permanente cada una (max 9) |
| **TOTAL** | | **~62** | |

★ Categorías nuevas en v2.1.

> **Cambio v3.1:** se reducen ~80→80 Destellos sueltos (sin cambio), ~12→10 Tés (reducidos con la linearización), ~20→17 Luces Cálidas, ~25→20 Ecos del Pasado. Los Fragmentos se reagrupan en **5 narrativos + 4 de Memoria Ambiental** (antes mezclados en 7 ambiguos). **Acero Mental** se removió del flujo principal.

**Inventario Emocional:** pantalla en el menú de pausa que muestra todos los coleccionables encontrados, el retrato emocional de Mateo, los Tés disponibles (stack), los fragmentos equipables (max 3), y el contador de Destellos seguros / en bolsa.

**Porcentaje de completitud** visible al terminar el juego — motivación para rejugabilidad y búsqueda del nivel secreto. Calculado sobre **rooms exploradas + coleccionables encontrados + Santuarios usados + Llave Oxidada encontrada + fragmentos narrativos reunidos**.

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
Cada nivel dura **10-15 minutos** de gameplay normal, con 10-15 rooms agrupadas en 3 actos. Diseño estrictamente lineal.

**8. Cada nivel tiene al menos un "momento pico" memorable.**
Una secuencia cinematográfica única de 30-60 segundos donde gameplay, narrativa, visual y audio convergen. Son los momentos que el jugador contará cuando hable del juego.

**9. Exploración lineal.**
El juego es 100% lineal sin ramas opcionales. Los coleccionables están integrados en la ruta principal para mantener el ritmo.

**10. Muerte nunca castiga de más, pero sí cuenta.**
**Bancas** frecuentes (~cada 3-5 rooms) actúan como respawn point inmediato sin guardar progreso. **Santuarios** (2-3 por nivel) son el verdadero refugio: heal completo, save real, depósito de Destellos, mapa del nivel. El fail state "**Desvanecerse**" (§13.c) te lleva al último Santuario activado y pierdes los Destellos no depositados — pero **no hay pantalla de Game Over**. El juego premia la persistencia, no la perfección. Y el sistema de mercy (Luz Cálida tras 3 muertes consecutivas en una room) garantiza que ningún jugador se quede atorado por orgullo.

## Progresión mecánica global

| Nivel | Habilidad nueva | Mecánica de sistema nueva | Duración | Rooms |
|---|---|---|---|---|
| **N1 — Umbral** | Doble salto, crouch, drop-through | Debuffs (Tunnel Vision), Ecos, alarmas, power-ups | 10-12 min | 14 |
| **N2 — Archivo** | Wall slide, wall jump, carry | Inversión de controles, clasificación de recuerdos | 10-12 min | 14 |
| **N3 — Mar Quieto** | Dash | Peso emocional (afecta velocidad), anclas sensoriales, banca 60s | 10-12 min | 14 |
| **N4 — Espejos** | Dash reset en wall jump (Voz del Niño) | Evasión pura, plano doble de gameplay, 1 Pregunta consolidada | 10-12 min | 14 |
| **N5 — Jardín** | Semilla de Luz | 5 memorias ambientales en secuencia, Mirador final | 10-12 min | 8 |
| **Secreto — Reencuentro** | (ninguna) | Walking simulator con Bruno | 8-10 min | 10 |

**Total de gameplay:** ~58-68 minutos (rooms × ~1 min/room + momentos pico).
**Total de rooms (incl. secreto):** **74** (reducido de ~152 en v2.1).

---

## Resumen narrativo de los 6 niveles

> **v3.0 — Linearización.** La arquitectura espacial detallada (dimensiones, coordenadas X/Y, diagramas ASCII, blueprints de anclas y clímax, transiciones exactas) de cada nivel vive en [`niveles_plano.md`](niveles_plano.md). Este documento conserva el **diseño narrativo y mecánico** de cada nivel: emoción, enemigos, habilidades, momentos pico, secretos, diálogos clave y transiciones. Cualquier cambio a un nivel debe replicarse en ambos documentos.
>
> **Total:** 74 rooms lineales (reducido de ~152 en v2.1). **Sin ramas de bifurcación** — la exploración se premia con secretos integrados a la ruta principal. Tiempo total estimado: 58-68 minutos.

### Tabla comparativa (ver [`niveles_plano.md` §7](niveles_plano.md) para detalle)

| Nivel | Emoción | Rooms | Habilidad nueva | Enemigos | Sant. | Bancas | Luces | Momento pico |
|---|---|---|---|---|---|---|---|---|
| **N1 — El Umbral** | Ansiedad | 14 | Doble salto, crouch, drop | Los Ecos | 1 | 2 | 3 | R1.14 — Sala del teléfono |
| **N2 — El Archivo** | Memoria/Confusión | 14 | Wall slide, wall jump, carry | Las Voces | 1 | 2 | 3 | R2.14 — Gran Mesa |
| **N3 — El Mar Quieto** | Depresión | 14 | Dash | Los Pesos | 2 | 1+60s | 3 | R3.10 — Banca de 60s |
| **N4 — Espejos Rotos** | Confrontación | 14 | Dash reset en wall jump | Fragmentos Rotos + La Sombra | 2 | 2 | 3 | R4.12 — Sala Circular |
| **N5 — El Jardín** | Integración | 8 | Semilla de Luz | Ninguno (La Raíz pasiva) | 1 | 0 | 5 | R5.7 — Mirador de la cima |
| **Secreto — Reencuentro** | Presencia | 10 | (ninguna) | Ninguno | 0 | 0 | 0 | R-S.5..R-S.8 — Conversación con Bruno |
| **TOTAL** | | **74** | | | **7** | **7+1** | **17** | |

---

## Nivel 1 — EL UMBRAL (Ansiedad)

> **Para arquitectura detallada (dimensiones, coordenadas, blueprints ASCII de R1.7 y R1.14):** ver [`niveles_plano.md` §1](niveles_plano.md).

### Emoción y arco
Desorientación → presión → clímax ansioso → silencio.

### Enemigos
**Los Ecos** (siluetas traslúcidas que persiguen; debuff Tunnel Vision 4s). Detalles en §7.

### Principio
*"Lo cotidiano se vuelve opresivo."* Aula vacía al amanecer → mundo industrial. Crescendo **ambiental** en Acto I (sin enemigos), **mecánico** en Acto II. **Tutorial completo del juego sin tutorial verbal** — todo se descubre por tentativa guiada.

### Estructura (14 rooms, 3 actos)
- **Acto I — Despertar (R1.1–R1.9, ~7 min):** Mateo despierta en el pupitre. Enseña caminar, saltar, primer Destello, frustrarse con la altura imposible. **R1.7 — Aura de la Púa** (ancla narrativa, desbloquea doble salto). R1.9 — primera Banca. R1.6 — Santuario 1 "La señal".
- **Acto II — Deformación (R1.10–R1.13, ~3 min):** aprende crouch (R1.10, techo bajo) y drop-through (R1.11, plataformas amarillas). Primera alarma industrial (R1.12). Enseña a leer el ciclo de peligro.
- **Acto III — La Sala del Teléfono (R1.14, ~1 min):** **MOMENTO PICO NARRATIVO.** Habitación silenciosa. 47 mensajes de Bruno apilados. Decisión **[E] RESPONDER / [Q] NO PUEDO** — ambas válidas, suman +10 Destellos, **afectan 1 Pregunta de La Sombra en N4** y el epílogo.

### Sistemas activos
- Enfoque: recarga **50% más lenta** (1.8s entre chispas). Capacidad 3.
- Aliento: 4 inicio, max 4.
- Peso: ambiental, rara vez >30%.
- Santuarios: 1 (R1.6 "La señal"). Bancas: 2 (R1.9, R1.13). Luces Cálidas: 3. Hazards duros: 0.
- Power-ups: Latido Calmado, Memoria Clara, Suspiro Profundo, Escudo de Respiración.

### Ancla narrativa
**R1.7 — Aura de la Púa**: cinemática 3s, texto *"Recuerdo estar emocionado por algo"*, desbloquea doble salto. Test inmediato en R1.8 (hueco 4 tiles).

### Diálogos clave
- R1.1: *"...otra mañana."*
- R1.3: *"Conozco este lugar. Estoy aquí todos los días. Y aun así me siento perdido."*
- R1.7: *"Recuerdo estar emocionado por algo. ¿Cuándo fue?"*
- R1.17: *"No sé por qué corro si la puerta siempre está igual de lejos."*
- R1.27: *"El reloj no marca horas. Marca las veces que no dije nada."*
- R1.30: *"(47 mensajes sin leer de Bruno.)"*

### Secretos (3)
Ver [`niveles_plano.md` §1.8](niveles_plano.md): Eco del Pasado en plataforma alta de R1.5, Destello entre one-way en R1.11, Destello+frase oculta en techo de R1.12.

### Transición a N2
Fade a blanco → *"No todo lo que cargas te pesa. Algunas cosas te sostienen."* (Eco Amable) → fade in al Archivo.

---

## Nivel 2 — EL ARCHIVO (Memoria y Confusión)

> **Para arquitectura detallada:** ver [`niveles_plano.md` §2](niveles_plano.md).

### Emoción y arco
Nostalgia → confusión → ternura incómoda → aceptación.

### Enemigos
**Las Voces** (bocas flotantes estáticas; debuff Controles Invertidos 3s al entrar en su zona de niebla púrpura). Detalles en §7.

### Principio
*"Las cosas que guardas te pesan o te sostienen."* Islas flotantes suspendidas en la oscuridad de la memoria. Mecánica única del nivel: **clasificación de recuerdos** (GUARDAR / SOLTAR / ENTENDER) que modifica el mundo — clasificar genera nuevas islas.

### Estructura (14 rooms, 3 actos)
- **Acto I — Entrar al Archivo (R2.1–R2.9, ~9 min):** Mateo aterriza tras N1. Aprende **carry** (R2.3, cargar púa/foto sobre la cabeza, −15% velocidad). R2.4 — primera clasificación (3 pedestales). R2.6 — primera Voz en el parallax (aún no peligrosa). R2.8 — aprende **wall slide** (pared alta, no puede subir todavía). **R2.9 — Santuario 1 "El cajón abierto"** + **primera aparición del Eco Amable** (diálogo + power-up gratis Foco de Claridad + comercio de Destellos abierto).
- **Acto II — Las Voces (R2.10–R2.13, ~3 min):** R2.10 — primera Voz en gameplay (primera inversión de controles, 3s). R2.11 — dos Voces contiguas (inversión encadenada). **R2.12 — Tinta de Estrellas** (ancla narrativa, desbloquea wall jump). R2.13 — test inmediato (3 paredes en zigzag).
- **Acto III — La Gran Mesa (R2.14, ~1 min):** **MOMENTO PICO NARRATIVO.** Sala circular amplia. 3 mesas enormes: Guardar / Soltar / Entender. 3 recuerdos a clasificar (púa, foto, nota). **27 combinaciones posibles**, cada una dispara un flashback único en el parallax (5s). **"ENTENDER × 3"** = bonus narrativo (Eco Amable único en N3). Combinación afecta **2 memorias ambientales de N5** + 1 Frase Oculta de N4.

### Sistemas activos
- Enfoque: recarga normal (1.2s). Capacidad 3.
- Aliento: 4-5 inicio. Peso: ambiental, +3% por cada inversión completa.
- Santuarios: 1 (R2.9). Bancas: 2 (R2.7, R2.10). Luces Cálidas: 3. Hazards duros: 0.
- Power-ups: Foco de Claridad, Silenciador, Suspiro Profundo, Escudo de Respiración.

### Ancla narrativa
**R2.12 — Tinta de Estrellas**: cinemática 3s con partículas estelares azules, Mateo flota, texto *"Recuerdo haber escrito un cuento sobre estrellas con nombre. Me trepaba a los árboles y les ponía nombres desde arriba"*, desbloquea wall jump.

### Diálogos clave
- R2.9 (Eco Amable): *"No todo lo que guardas te pesa. Algunas cosas te sostienen."*
- R2.10 (Voz): *"Eso no pasó así."*
- R2.18: *"La radio nunca termina la canción. Igual que yo."*
- R2.25: *"Hay cosas que no recuerdo haber guardado. Pero ahí están, ocupando espacio en mí."*
- R2.27: *"No tiré nada. Solo dejé de mirar. Y de alguna forma eso fue peor."*

### Secretos (3)
Ver [`niveles_plano.md` §2.8](niveles_plano.md): Destello en pared lateral de R2.5, Eco del Pasado en cima de pared alta R2.8 (wall slide + doble salto), Eco Amable único en N3 R3.0 si clasificaste 3× "ENTENDER".

### Transición a N3
Puente dorado → borde del archivo → Mateo se asoma al abismo gris → cae en slow motion (parallax se desatura a monocromo) → fade in al Mar Quieto con silencio ambiental.

---

## Nivel 3 — EL MAR QUIETO (Depresión)

> **Para arquitectura detallada:** ver [`niveles_plano.md` §3](niveles_plano.md).

### Emoción y arco
Vacío → peso → rendición → silencio sanador → ascenso.

### Enemigos
**Los Pesos** (esferas oscuras adherentes, pasivos, no persiguen; debuff Pasos de Plomo 5s — salto al 50%, velocidad al 70%). Detalles en §7.

### Principio
*"Bajar es a veces el único camino."* **Único nivel principalmente vertical** — Mateo cae por 2 actos, luego asciende en el tercero. El dash representa *"atreverte a moverte cuando todo pesa"*: sus frames de invulnerabilidad permiten atravesar Pesos sin adherirlos (metáfora visual clara).

**Cambio v3.0:** estructura vertical simplificada a 2 tramos claros (descenso R3.1–R3.7, banca R3.10, ascenso R3.11–R3.14). La **Rama N3.A "Fondo Luminoso"** se eliminó en la linearización; la **Llave Oxidada** se movió al Nivel Secreto R-S.1.

### Estructura (14 rooms, 3 actos)
- **Acto I — Caer (R3.1–R3.7, ~5 min):** Mateo cae 5s en slow motion a través del parallax. **R3.2 — Santuario 1 "La flor que respira"** (cama flotante; aparece la **barra UI de Peso Emocional vertical, lado izquierdo** — única del juego, empieza en 30%). Aprende Anclas Sensoriales (R3.4 flor, R3.6 roca): cada Ancla cura +1 fragmento de Aliento y baja 15% de peso. **R3.5 — Hazard duro:** abismo central con tinte rojo (Desvanecerse instantáneo, no −1 fragmento). R3.7 — Banca 1 (sin rama a Fondo Luminoso, eliminado).
- **Acto II — La Banca (R3.8–R3.10, ~4 min):** **R3.8 — Chispa de Atreverse** (ancla narrativa, desbloquea dash). R3.9 — primer hueco dashable (6 tiles, solo dash). **R3.10 — La Banca de 60s: MOMENTO PICO ANTI-GAMEPLAY.** 60 segundos reales sentado sin input. HUD desaparece. Música se apaga. Secuencia: 0-10s silencio absoluto → 10-20s parallax gris→azul → 20-35s nota de piano cada 3s + flashbacks → 35-50s melodía, cielo→madrugada → 50-60s crescendo, amanecer. **Texto: *"Descansar también es avanzar."* Peso → 0%. Mundo se colorea.** Sin skip. Sin input.
- **Acto III — Subir (R3.11–R3.14, ~3 min):** **R3.11 — Santuario 3 "La flor renacida"** (paralelo visual a R3.2, ahora con luciérnagas). R3.12 — Ancla del Oído (caracol marino, canción de cuna). R3.13 — Botella de Bruno (*"Yo también estuve aquí. Respira. — B."*). R3.14 — emersión al amanecer, *"Descansar también es avanzar"* reaparece como texto fantasma, fade a blanco.

### Sistemas activos
- Enfoque: recarga normal (1.2s) con peso <30%. **Máximo baja a 2 chispas con peso >50%** (vuelve a 3 al usar Ancla).
- Aliento: 4-5 inicio. Las Anclas curan +1.
- Peso: **único nivel donde llega a 91-100%**. Barra UI explícita solo aquí.
- Santuarios: 2 (R3.2, R3.11). Bancas: 1 + 60s. Luces Cálidas: 3. Hazards duros: 1.
- Power-ups: Burbuja de Aire, Corriente Cálida, Luz Interior, Suspiro Profundo, Escudo de Respiración.
- **Llave Oxidada:** ya no se encuentra aquí. Se obtiene en el Secreto R-S.1.

### Ancla narrativa
**R3.8 — Chispa de Atreverse**: Mateo da un paso adelante, vibra (1 frame de screen shake), luz blanca inunda la pantalla (1s), texto *"Recuerdo haber saltado de un muro sin mirar"*, desbloquea dash. Frames de invulnerabilidad del dash (0.18s) permiten atravesar Pesos.

### Diálogos clave
- R3.5: *"Olor a... algo que recuerdo."*
- R3.6: *"No tengo miedo. No tengo nada. Y eso es lo más extraño que he sentido."*
- R3.8: *"Recuerdo haber saltado de un muro sin mirar."*
- R3.10 (banca): *"Descansar también es avanzar."*
- R3.13: *"Yo también estuve aquí. Respira. — B."*
- R3.20: *"Perdón por no haberte preguntado antes."*

### Secretos (3)
Ver [`niveles_plano.md` §3.8](niveles_plano.md): Eco del Pasado gris-rojo en R3.6 (recogerlo sube peso +10%, intencional), Destello en cima del caracol R3.12 (timing con doble salto), cinemática extra con silueta de Bruno al lado si completas Acto III con peso=0.

### Transición a N4
Mateo camina por la superficie iluminada. A lo lejos, un edificio circular con ventanas espejadas. Se acerca. Su propio reflejo lo mira desde una ventana — pero el reflejo **no lo imita**. Fade a rojo oscuro.

---

## Nivel 4 — ESPEJOS ROTOS (Confrontación)

> **Para arquitectura detallada:** ver [`niveles_plano.md` §4](niveles_plano.md).

### Emoción y arco
Inquietud → reconocimiento → miedo → aceptación → ternura.

### Enemigos
**Fragmentos Rotos** (pedazos de espejo orbitales; debuff Flash de Recuerdo 1s — pantalla blanca + imagen flash de recuerdo feliz). **La Sombra** (en parallax imitando/anticipando a Mateo, más 1 Pregunta consolidada + Confrontación final). Detalles en §6 y §7.

### Principio
*"Encontrarte a ti mismo es el miedo más grande."* **Nivel de consolidación mecánica** — no introduce habilidades de movimiento nuevas, pero es el más denso narrativamente. Mecánica única del nivel: **"Diálogo tras combate"** (R4.9) — La Sombra pregunta, el jugador esquiva Fragmentos Rotos mientras elige.

**Cambio v3.0:** las 3 "Preguntas de La Sombra" de v2.1 (R4.9, R4.16, R4.19) se consolidan en **1 sola Pregunta** (R4.9) con más peso narrativo. La mecánica se preserva intacta pero aparece una vez.

### Estructura (14 rooms, 3 actos)
- **Acto I — La Galería (R4.1–R4.7, ~5 min):** Mateo entra a un edificio circular. Espejos perturbadores: vacío (R4.1), con 1s de retraso (R4.2), en dirección opuesta (R4.3). **R4.4 — Santuario 1 "El reflejo entero"** (único espejo sin grieta con marco dorado; La Sombra no aparece en su radio). R4.5 — primeros Fragmentos Rotos orbitales. R4.6 — Banca 1 donde La Sombra habla por primera vez (*"Si no intentas nada, nadie puede decepcionarte."*). **R4.7 — Voz del Niño** (ancla narrativa, dash reset en wall jump — habilita cadenas dash→WJ→dash→WJ).
- **Acto II — El Puente de Reflejos (R4.8–R4.10, ~3 min):** R4.8 — circuito de dash+wall jump (test del fragmento). **R4.9 — La Pregunta (DIÁLOGO TRAS COMBATE CONSOLIDADA):** La Sombra se materializa en gameplay. 6 Fragmentos orbitan alrededor. Mateo esquiva 12s + elige entre 3 opciones:
  - **[1] Honesta** (texto varía según flag de N1: *"Porque tenía miedo"* o *"Porque no quería sentirme peor"*)
  - **[2] Evasiva** (*"Porque no sabía qué decir"*)
  - **[3] Empática**
  - **Silencio** (default si no elige a tiempo).
  - **Efecto mecánico:** Honesta desactiva 50% de Fragmentos en R4.10, Evasiva 0%, Empática 75%, Silencio 25%. Las 4 son válidas. **R4.10 — Banca 2 "abrazo"** (Mateo se abraza 5s).
- **Acto III — La Confrontación (R4.11–R4.14, ~3 min):** **R4.11 — Santuario 2 "El reflejo entero II"** (último save antes del clímax; texto *"Antes de cruzar, respira."*). **R4.12 — Sala Circular: MOMENTO PICO NARRATIVO.** Sin Fragmentos por 1ª vez. Sin enemigos. La Sombra en el centro. Diálogo largo. Decisión final **[1] SÍ / [2] NO / [3] NO LO SÉ** — todas válidas:
  - **Sí:** La Sombra camina al lado de Mateo en N5 (sprite compañero visible todo el nivel). Final "Acompañado".
  - **No lo sé:** La Sombra se desvanece pero susurra *"Te escucho, cuando me necesites."* Final "Pendiente".
  - **No:** La Sombra se desvanece silenciosa. N5 tiene tono más introspectivo. Final "Solo".
  - R4.13 — sala con luz cálida, puerta verde aparece. R4.14 — fade a verde cálido, jardín en horizonte.

### Sistemas activos
- Enfoque: recarga normal pero **drenaje −1 chispa al recibir Flash de Recuerdo**.
- Aliento: 5-6 fragmentos esperados al llegar.
- Peso: +10% por Pregunta evasiva. Honesta/empática lo deja igual.
- Santuarios: 2 (R4.4, R4.11). Bancas: 2 (R4.6, R4.10). Luces Cálidas: 3. Hazards duros: 1 (R4.9, fosa de cristales).
- Power-ups: Escudo de Verdad, Voz Interior, Suspiro Profundo, Escudo de Respiración.

### Ancla narrativa
**R4.7 — Voz del Niño**: trompo de madera infantil girando. Cinemática con cámara 360° alrededor del trompo, texto *"Recuerdo que me tiraba de los árboles sin pensar que me podía romper"*, habilita dash reset en wall jump.

### Diálogos clave
- R4.6 (Sombra): *"Si no intentas nada, nadie puede decepcionarte."*
- R4.9 (Sombra): *"Dime algo. ¿Por qué contestaste el teléfono?"* (si respondió en N1) o *"¿Por qué no contestaste? ¿Por qué lo dejaste esperando?"* (si no respondió).
- R4.12 (Sombra): *"Aprendí a quedarme quieta para que no te doliera."* / *"Pero me convertí en todas las cosas que no dijiste."* / *"¿Me dejarías ir contigo? No para que te cuide. Para que nos acompañemos."*

### Secretos (3)
Ver [`niveles_plano.md` §4.9](niveles_plano.md): Eco del Pasado en R4.5 (wall jump + dash), Frase oculta *"Me quería como era"* tras el trompo en R4.7 (sentarse 10s), sprite compañero de La Sombra en N5 si eligió "Sí" en R4.12.

### Transición a N5
La puerta verde se abre (primer verde del nivel). Mateo (y La Sombra si eligió Sí) caminan hacia un jardín en el horizonte. **Música de guitarra completa** suena por primera vez en el juego. Fade in al Jardín.

---

## Nivel 5 — EL JARDÍN (Integración)

> **Para arquitectura detallada:** ver [`niveles_plano.md` §5](niveles_plano.md).

### Emoción y arco
Calma → alegría → melancolía → plenitud → dignidad.

### Enemigos
**Ninguno.** La Raíz aparece solo si no avanzas 15s (advertencia suave, no daña). El jardín es completamente seguro.

### Principio
*"Volver a casa, pero cambiado."* Nivel de celebración y cierre. La dificultad mecánica baja — ya no se trata de probar habilidad, sino de **usar libremente todo lo aprendido con alegría**.

**Cambio v3.1:** la estructura se mantiene lineal, pero las **5 memorias del pasado** dejan de requerir NPCs físicos de Mateo joven. Ahora son **memorias ambientales**: objetos, luces, sonidos y pequeñas escenas del jardín. El Eco Amable guía cada memoria y entrega las recompensas; La Sombra acompaña en silencio si fue aceptada en N4.

**Twist:** la **Semilla de Luz** (entregada en R5.1) **modifica el entorno permanentemente** — primera mecánica del juego donde el jugador *crea* mundo en vez de atravesarlo. Narrativa: *"empiezas a dejar marca propia"*.

**Nota:** si elegiste "Sí" en N4 R4.12, La Sombra **acompaña todo el nivel** como sprite compañero visible. Su animación es simple pero presente (se detiene cuando Mateo se detiene, mira en la misma dirección). En R5.6 y R5.7 puede acercarse a Mateo como gesto de reconciliación, sin hablar.

### Estructura (8 rooms, lineal)
- **R5.1 — La llegada (SANTUARIO 1 "Donde crece"):** Mateo cruza un arco de piedra cubierto de enredaderas. Árbol joven con linternas de papel colgando. Eco Amable entrega la 1ª Semilla de Luz. Texto: *"Llegaste."*
- **R5.2 — La Luz (7 años / Alegría Sin Razón):** rayuela de luz, hojas girando, risa lejana y un camino corto que invita a correr sin presión. Eco Amable entrega **Fragmento "Alegría Sin Razón"** (+8% velocidad) + **Galleta 1** (+1 Aliento max). *"No te olvides de esto. De correr sin razón."*
- **R5.3 — El Ritmo (10 años / Última Guitarra):** guitarra apoyada en una raíz, notas flotantes y un puente musical que se enciende con la Semilla de Luz. Entrega **Fragmento "Última Guitarra"** (5º narrativo, da estado "Integración") + **Galleta 2**. *"No dejes de tocar."*
- **R5.4 — El Silencio (12 años / Raíz Cálida):** cuaderno abierto, luciérnagas quietas y una pausa breve de 5-8s donde el viento baja de volumen. No hay banca. Entrega **Fragmento "Raíz Cálida"** + **Galleta 3**. *"Pensar no es malo. Solo cansa si nadie escucha."*
- **R5.5 — La Carrera (14 años / Ancla del Silencio):** corredor de hojas, huellas luminosas que aparecen al lado de Mateo y una Semilla de Luz que crea el último tramo. Si La Sombra acompaña, camina/corre en paralelo. Entrega **Fragmento "Ancla del Silencio"** + **Galleta 4**. *"No siempre corrías para huir. A veces corrías porque estabas vivo."*
- **R5.6 — La Conversación (16 años / Voz del Niño o Perdón):** cuarto simbólico dentro del jardín: ventana iluminada, cassette/teléfono apagado y luz cálida baja. Eco Amable guía la conversación; La Sombra se acerca si está presente. Entrega **Voz del Niño** (si no la tienes) o **Perdón** (si ya la tienes) + **Galleta 5**. *"La parte que querías dejar atrás también estaba intentando cuidarte."*
- **R5.7 — El mirador de la cima (CLÍMAX FINAL):** los 5 recuerdos aparecen como objetos/luces en orden: rayuela, guitarra, cuaderno, huellas, ventana/cassette. Mateo se sienta al centro; La Sombra compañera (si está) se sienta a su lado. Halo de Aliento refleja las 5 Galletas (max 9 fragmentos). Texto: *"No volví solo. Volví con todo lo que fui."* Música de guitarra llega a su clímax. Las cinco luces se fusionan con Mateo. **Sprite cambia: retrato de silueta → rostro completo. Estado cosmético "Integración" desbloqueado.**
- **R5.8 — Créditos:** fade a blanco, créditos sobre parallax del amanecer, música de guitarra completa.

### Epílogos (según elecciones)
- **Si RESPONDER en N1 R1.14 + ≥7 ENTENDER en N2 R2.14:** cinemática alternativa (Mateo marca el teléfono de Bruno, "Hola.", "Hola, Bruno.", fade).
- **Si tiene la Llave Oxidada (encontrada en Secreto R-S.1):** mensaje *"Hay una banca esperando. ¿Quieres visitarla?"* → abre el Nivel Secreto.
- **100% completitud:** combina ambos epílogos.
- **Mensaje final obligatorio (todos los finales):** pantalla negra con línea de crisis y *"Pedir ayuda no es rendirse."*

### Sistemas activos
- Enfoque: recarga **30% más rápida** (0.85s). Capacidad 3. Mateo está en paz.
- Aliento: 4-9 (4 base + 5 Galletas de Memoria).
- Peso: baja constantemente. Casi imposible de subir.
- Santuarios: 1 (R5.1). Bancas: 0. Luces Cálidas: 5 (1 entre cada Memoria). Hazards duros: 0.
- Power-ups: Semilla de Luz, Eco de Música, Suspiro Profundo, Escudo de Respiración.

### Diálogos clave
- R5.1 (Eco Amable): *"Llegaste."* / *"No necesitas verlos para saber que siguen contigo."*
- R5.2 (Eco Amable): *"Aquí corrías sin pedir permiso al mundo. Come algo: también se vuelve a casa por el cuerpo."*
- R5.3 (Eco Amable): *"Antes de que doliera hablar, todavía sabías sonar. No dejes de tocar."*
- R5.4 (Eco Amable): *"Pensar no es malo. Solo cansa si nadie escucha. Hoy sí hay alguien escuchando."*
- R5.5 (Eco Amable): *"No siempre corrías para huir. A veces corrías porque estabas vivo."*
- R5.6 (Eco Amable): *"La parte que querías dejar atrás también estaba intentando cuidarte."*
- R5.7 (Mateo): *"No volví solo. Volví con todo lo que fui."*

### Secretos (3)
Ver [`niveles_plano.md` §5.7](niveles_plano.md): Frase oculta *"El silencio también era amor"* en R5.4, Destello+fragmento cosmético si La Sombra se acerca en R5.6, ventana iluminada de Bruno en parallax de R5.7 (solo si eligió SÍ en N4 + Semilla en borde correcto).

---

## Nivel Secreto — EL REENCUENTRO (Bruno)

> **Para arquitectura detallada:** ver [`niveles_plano.md` §6](niveles_plano.md).

### Acceso
Solo si el jugador tiene la **Llave Oxidada**. Aparece tras los créditos de N5 (o desde menú principal) con el mensaje *"Hay una banca esperando. ¿Quieres visitarla?"* con opción **[E] Visitar**. Si elige no, el juego termina normal y la opción queda disponible desde el menú principal.

**Cambio v3.0:** la Llave Oxidada ya **no se encuentra en N3** (la Rama N3.A "Fondo Luminoso" fue eliminada en la linearización). Ahora se obtiene en la **primera sala R-S.1** del propio nivel secreto, simplificando el flujo. Esto preserva la emoción de la Llave como "recompensa por cuidar", pero la hace accesible al primer playthrough.

### Enemigos
**Ninguno.** Es un walking simulator intencional.

### Principio
*"Habla con alguien. Siempre hay alguien al otro lado."* El nivel secreto es la **antítesis mecánica** del resto del juego. No hay habilidades mecánicas. No hay peligro. **No hay dash, ni salto, ni power-ups.** Solo caminar y escuchar. Todas las herramientas que el jugador acumuló se vuelven irrelevantes para forzar la emoción de *"estar y nada más"*. La razón: después de 2+ horas de platforming avanzado, reducir al jugador a un solo verbo **fuerza mecánicamente** la emoción que el nivel quiere provocar.

### Estructura (10 rooms, lineal)
- **R-S.1 — La puerta oxidada:** Mateo abre puerta con la Llave Oxidada (que se encuentra flotando al centro de la sala). Fade a atardecer.
- **R-S.2 a R-S.4 — El sendero:** 2-3 min de caminata sin eventos. No hay HUD. Solo A/D funciona. Intentar saltar muestra *"No hay prisa."* Mateo 12 años aparece fugaz en parallax mid.
- **R-S.5 — Sentarse (MOMENTO PICO):** 30s de silencio real. Bruno se quita un audífono.
- **R-S.6 — La conversación:** diálogo automático, sin opciones, 7 intercambios (Ey → Creí que te había perdido → Perdón por no contestar → Perdón por no preguntar → pausa larga 8s).
- **R-S.7 — El flashback:** 2 niños con guitarras, canción completa por 1ª vez (10s).
- **R-S.8 — El silencio compartido:** 60s de silencio real. Sol baja, luciérnagas. Texto *"El silencio también puede ser amor, si lo compartes."*
- **R-S.9 — El cassette (opcional):** si encontró el cassette en N2 R2.19, Bruno saca cassette player. Canción adicional de 30s.
- **R-S.10 — El cierre:** fade a negro 15s, mensaje final + línea de crisis. Juego se cierra auto a 30s. No hay botón de continuar.

### Sistemas activos (todos deshabilitados)

| Sistema | Estado | Detalle |
|---|---|---|
| Enfoque | DESHABILITADO | Sin chispas, sin recarga |
| Aliento | INMORTAL | Halo en oro absoluto, no daña |
| Peso Emocional | DESHABILITADO | Niebla invisible |
| Bancas / Santuarios / Luces Cálidas | 0 | No necesarios |
| Habilidades | DESHABILITADAS | Solo caminar |
| Save | AUTO al inicio y al final | Como flag, no como save de sub-room |
| Fail state | INEXISTENTE | No se puede Desvanecerse |
| Inventario | ACTIVO | Pero no se puede usar |

### Diálogo principal (R-S.6, sin opciones, sin skip)
```
Bruno: "Ey."
Mateo: "Ey."
Bruno: "¿Hace cuánto no nos veíamos?"
Mateo: "No sé. Mucho."
Bruno: "Creí que te había perdido."
Mateo: "Yo también creí que te había perdido."
Bruno: "Yo también estuve mal. No sabía cómo decirlo."
Mateo: "Yo tampoco."
Bruno: "Perdón por no contestar."
Mateo: "Perdón por no preguntar."
```
(Pausa larga 8s → R-S.7 flashback.)

### Mensaje final obligatorio (R-S.10)
*"Habla con alguien. Siempre hay alguien al otro lado. Si tú o alguien que conoces está luchando, no estás solo. Busca a un adulto de confianza, un amigo, un profesional. Pedir ayuda no es rendirse. Es el acto más valiente que puedes hacer."*

**Línea de Crisis Psicológica (Perú):** Línea 113 (MINSA) — Opción 5. Disponible 24/7, gratuita, confidencial.

El juego se cierra automáticamente tras 30 segundos. No hay botón de continuar.

### Nota de diseño del equipo

El nivel secreto **no es un desafío**. Es una recompensa. No por dominar el juego, sino por **cuidarlo**. La Llave Oxidada se encuentra al inicio del propio nivel, haciéndolo accesible al primer playthrough — pero el nivel mismo recompensa la presencia emocional. El juego premia **estar con alguien**, no la habilidad.

Esta es la declaración de intenciones final del proyecto: **pedir ayuda salva vidas. Estar con alguien en silencio también.**

---# V. ARTE Y AUDIO

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

Detallada en [PROGRESO.md](PROGRESO.md). Namespace root: `Inward`. Sub-namespaces por dominio. Los scripts marcados ★ son nuevos en v2.1 (Sprint 1 planeado):

```
Inward.Core          → GameManager, ObjectPool, AudioManager, GameEvents
Inward.Player        → PlayerController2D, PlayerHealth, IdentityManager,
                        ★ FocusSystem, ★ EmotionalWeight
Inward.Systems       → PowerUpManager, DebuffManager, Collectible,
                        ★ Bench (renombrado de Checkpoint),
                        ★ SanctuarySystem, ★ WarmLight, ★ TeaItem
Inward.CameraSystem  → CameraFollow2D, ParallaxLayer
Inward.AI            → EnemyBase, WalkerEnemy, FlyerEnemy
Inward.Levels        → LevelManager, Level01Manager, OneWayPlatform, Hazard
Inward.UI            → HUDManager (refactor a HUD diegético), PauseMenu
Inward.Utils         → Placeholders, SceneBootstrapper
Inward.ScriptableObjects → IdentityFragmentSO, PowerUpSO, DebuffSO,
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

**Última actualización del documento:** 2026-06-04 — Linearización v3.0: reducción de ~152 rooms a 74 lineales (sin ramas). N3 vertical simplificado, N4 con 1 sola Pregunta consolidada, N5 con 5 Memorias en secuencia (hub-and-spokes eliminado). Llave Oxidada movida al Secreto. Aliento max unificado a 9. Inconsistencias de §8.1 resueltas. La arquitectura espacial detallada de cada nivel ahora vive en [`niveles_plano.md`](niveles_plano.md); este documento conserva la definición de sistemas, mecánicas, personajes, narrativa y guía de implementación.

**Estado técnico actual:** Core + Nivel 1 prototipo en construcción (Sprint 0). La implementación de los sistemas v2.1 está planeada para el Sprint 1. Ver [PROGRESO.md](PROGRESO.md).
