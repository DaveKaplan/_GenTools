using WebFrame;

namespace Tools.Pages;

public class SpanishPage : StaticPage
{
    public override string Title => "Spanish Vocabulary";
    public override string Route => "/Tools/Spanish";

    protected override string HtmlContent => """
        <style>
          .btn-group { display: flex; gap: .75rem; margin-bottom: 1.5rem; flex-wrap: wrap; }
          .btn {
            padding: .6rem 1.8rem; font-size: 1rem; font-weight: 600;
            border: 2px solid #1a3c5e; border-radius: 6px;
            background: #fff; color: #1a3c5e; cursor: pointer; transition: background .15s, color .15s;
          }
          .btn:hover  { background: #1a3c5e; color: #fff; }
          .btn.active { background: #1a3c5e; color: #fff; }

          .word-table { display: none; width: 100%; border-collapse: collapse; }
          .word-table.visible { display: table; }
          .word-table th {
            background: #1a3c5e; color: #fff;
            padding: .6rem 1rem; text-align: left; font-size: .95rem;
          }
          .word-table td { padding: .45rem .75rem; border-bottom: 1px solid #e2e8f0; vertical-align: middle; }
          .word-table tr:last-child td { border-bottom: none; }
          .word-table tr:nth-child(even) td { background: #f7fafc; }

          .speak-btn {
            background: none; border: none; cursor: pointer;
            font-size: 1.1rem; padding: 0 .25rem; line-height: 1;
            opacity: .6; transition: opacity .15s, transform .1s;
          }
          .speak-btn:hover  { opacity: 1; }
          .speak-btn.speaking { opacity: 1; transform: scale(1.25); }

          .qt-box {
            background: #fff; border: 1px solid #dde3ea; border-radius: 8px;
            padding: 1rem 1.25rem; margin-bottom: 1.5rem; display: flex; flex-direction: column; gap: .65rem;
          }
          .qt-box label { font-weight: 600; color: #1a3c5e; font-size: .9rem; }
          .qt-row { display: flex; gap: .5rem; align-items: center; flex-wrap: wrap; }
          .qt-input {
            flex: 1; min-width: 180px; max-width: 100%;
            padding: .45rem .75rem; font-size: .95rem;
            border: 1px solid #ccd3db; border-radius: 6px;
          }
          .qt-dir {
            padding: .4rem .9rem; font-size: .85rem; font-weight: 600; cursor: pointer;
            border: 2px solid #1a3c5e; border-radius: 6px;
            background: #fff; color: #1a3c5e; white-space: nowrap;
          }
          .qt-dir:hover { background: #e8edf2; }
          .qt-btn {
            padding: .4rem 1.1rem; font-size: .9rem; font-weight: 600; cursor: pointer;
            background: #1a3c5e; color: #fff; border: none; border-radius: 6px;
          }
          .qt-btn:hover { background: #265a8a; }
          .qt-btn:disabled { opacity: .5; cursor: default; }
          .qt-output {
            min-height: 2rem; padding: .45rem .75rem;
            background: #f0f4f8; border-radius: 6px; font-size: .95rem; color: #222;
            display: none;
          }
          .qt-output.visible { display: block; }

          .voice-selector { display: flex; align-items: center; gap: .5rem; margin-bottom: 1.25rem; font-size: .95rem; }
          .voice-selector label { font-weight: 600; color: #1a3c5e; margin-right: .25rem; }
          .voice-btn {
            padding: .35rem 1.1rem; font-size: .9rem; font-weight: 600; cursor: pointer;
            border: 2px solid #1a3c5e; border-radius: 6px;
            background: #fff; color: #1a3c5e; transition: background .15s, color .15s;
          }
          .voice-btn.active { background: #1a3c5e; color: #fff; }
          .voice-btn:hover:not(.active) { background: #e8edf2; }

          .count-label { color: #666; font-size: .85rem; margin-bottom: .5rem; }
          a.back { display: inline-block; margin-top: 1.5rem; color: #1a3c5e; }
        </style>

        <!-- QUICK TRANSLATE -->
        <div class="qt-box">
          <label>Traducción rápida</label>
          <div class="qt-row">
            <input id="qt-input" class="qt-input" type="text" maxlength="100"
                   placeholder="Escribe aquí..." oninput="qtCharCount()" onkeydown="if(event.key==='Enter')qtTranslate()" />
            <button class="speak-btn" onclick="qtSpeakInput(this)" title="Pronunciar entrada">🔊</button>
            <button class="qt-dir" id="qt-dir" onclick="qtToggleDir()" title="Cambiar dirección">
              Inglés → Español
            </button>
            <button class="qt-btn" id="qt-btn" onclick="qtTranslate()">Traducir</button>
          </div>
          <div class="qt-row" style="align-items:flex-start; gap:.5rem;">
            <div id="qt-output" class="qt-output" style="flex:1"></div>
            <button class="speak-btn" id="qt-speak-out" onclick="qtSpeakOutput(this)"
                    title="Pronunciar traducción" style="display:none; margin-top:.3rem;">🔊</button>
          </div>
        </div>

        <div class="voice-selector">
          <label>Voice:</label>
          <button class="voice-btn active" id="voice-f" onclick="setVoice('f',this)">&#9792; Femenino (Dalia)</button>
          <button class="voice-btn"        id="voice-m" onclick="setVoice('m',this)">&#9794; Masculino (Jorge)</button>
        </div>

        <div class="btn-group">
          <button class="btn" onclick="show('nouns', this)">Sustantivos</button>
          <button class="btn" onclick="show('verbs', this)">Verbos</button>
          <button class="btn" onclick="show('numbers', this)">Números</button>
          <button class="btn" onclick="show('random', this)">Aleatorio</button>
        </div>

        <div id="count-label" class="count-label"></div>

        <!-- NOUNS -->
        <table id="nouns" class="word-table">
          <thead><tr><th>Inglés</th><th>Español</th><th></th></tr></thead>
          <tbody>
            <tr><td>house</td><td>la casa</td><td><button class="speak-btn" onclick="speak('la casa',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>dog</td><td>el perro</td><td><button class="speak-btn" onclick="speak('el perro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>cat</td><td>el gato</td><td><button class="speak-btn" onclick="speak('el gato',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>water</td><td>el agua</td><td><button class="speak-btn" onclick="speak('el agua',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>food</td><td>la comida</td><td><button class="speak-btn" onclick="speak('la comida',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>man</td><td>el hombre</td><td><button class="speak-btn" onclick="speak('el hombre',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>woman</td><td>la mujer</td><td><button class="speak-btn" onclick="speak('la mujer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>child</td><td>el niño / la niña</td><td><button class="speak-btn" onclick="speak('el niño',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>city</td><td>la ciudad</td><td><button class="speak-btn" onclick="speak('la ciudad',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>school</td><td>la escuela</td><td><button class="speak-btn" onclick="speak('la escuela',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>book</td><td>el libro</td><td><button class="speak-btn" onclick="speak('el libro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>car</td><td>el coche</td><td><button class="speak-btn" onclick="speak('el coche',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>day</td><td>el día</td><td><button class="speak-btn" onclick="speak('el día',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>night</td><td>la noche</td><td><button class="speak-btn" onclick="speak('la noche',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>family</td><td>la familia</td><td><button class="speak-btn" onclick="speak('la familia',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>friend</td><td>el amigo / la amiga</td><td><button class="speak-btn" onclick="speak('el amigo',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>work</td><td>el trabajo</td><td><button class="speak-btn" onclick="speak('el trabajo',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>money</td><td>el dinero</td><td><button class="speak-btn" onclick="speak('el dinero',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>hand</td><td>la mano</td><td><button class="speak-btn" onclick="speak('la mano',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>time</td><td>el tiempo</td><td><button class="speak-btn" onclick="speak('el tiempo',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>door</td><td>la puerta</td><td><button class="speak-btn" onclick="speak('la puerta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>table</td><td>la mesa</td><td><button class="speak-btn" onclick="speak('la mesa',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>chair</td><td>la silla</td><td><button class="speak-btn" onclick="speak('la silla',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>window</td><td>la ventana</td><td><button class="speak-btn" onclick="speak('la ventana',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>street</td><td>la calle</td><td><button class="speak-btn" onclick="speak('la calle',this)" title="Pronunciar">🔊</button></td></tr>
          </tbody>
        </table>

        <!-- VERBS -->
        <table id="verbs" class="word-table">
          <thead><tr><th>Inglés</th><th>Español (infinitivo)</th><th></th></tr></thead>
          <tbody>
            <tr><td>to be (permanent)</td><td>ser</td><td><button class="speak-btn" onclick="speak('ser',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to be (temporary)</td><td>estar</td><td><button class="speak-btn" onclick="speak('estar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to have</td><td>tener</td><td><button class="speak-btn" onclick="speak('tener',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to do / to make</td><td>hacer</td><td><button class="speak-btn" onclick="speak('hacer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to go</td><td>ir</td><td><button class="speak-btn" onclick="speak('ir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to say / to tell</td><td>decir</td><td><button class="speak-btn" onclick="speak('decir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to see</td><td>ver</td><td><button class="speak-btn" onclick="speak('ver',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to want</td><td>querer</td><td><button class="speak-btn" onclick="speak('querer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to come</td><td>venir</td><td><button class="speak-btn" onclick="speak('venir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to know (facts)</td><td>saber</td><td><button class="speak-btn" onclick="speak('saber',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to know (people/places)</td><td>conocer</td><td><button class="speak-btn" onclick="speak('conocer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to speak</td><td>hablar</td><td><button class="speak-btn" onclick="speak('hablar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to eat</td><td>comer</td><td><button class="speak-btn" onclick="speak('comer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to drink</td><td>beber</td><td><button class="speak-btn" onclick="speak('beber',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to sleep</td><td>dormir</td><td><button class="speak-btn" onclick="speak('dormir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to work</td><td>trabajar</td><td><button class="speak-btn" onclick="speak('trabajar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to live</td><td>vivir</td><td><button class="speak-btn" onclick="speak('vivir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to think</td><td>pensar</td><td><button class="speak-btn" onclick="speak('pensar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to give</td><td>dar</td><td><button class="speak-btn" onclick="speak('dar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to take</td><td>tomar</td><td><button class="speak-btn" onclick="speak('tomar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to read</td><td>leer</td><td><button class="speak-btn" onclick="speak('leer',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to write</td><td>escribir</td><td><button class="speak-btn" onclick="speak('escribir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to open</td><td>abrir</td><td><button class="speak-btn" onclick="speak('abrir',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to close</td><td>cerrar</td><td><button class="speak-btn" onclick="speak('cerrar',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>to buy</td><td>comprar</td><td><button class="speak-btn" onclick="speak('comprar',this)" title="Pronunciar">🔊</button></td></tr>
          </tbody>
        </table>

        <!-- NUMBERS -->
        <table id="numbers" class="word-table">
          <thead><tr><th>#</th><th>Español</th><th></th></tr></thead>
          <tbody>
            <tr><td>0</td><td>cero</td><td><button class="speak-btn" onclick="speak('cero',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>1</td><td>uno</td><td><button class="speak-btn" onclick="speak('uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>2</td><td>dos</td><td><button class="speak-btn" onclick="speak('dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>3</td><td>tres</td><td><button class="speak-btn" onclick="speak('tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>4</td><td>cuatro</td><td><button class="speak-btn" onclick="speak('cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>5</td><td>cinco</td><td><button class="speak-btn" onclick="speak('cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>6</td><td>seis</td><td><button class="speak-btn" onclick="speak('seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>7</td><td>siete</td><td><button class="speak-btn" onclick="speak('siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>8</td><td>ocho</td><td><button class="speak-btn" onclick="speak('ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>9</td><td>nueve</td><td><button class="speak-btn" onclick="speak('nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>10</td><td>diez</td><td><button class="speak-btn" onclick="speak('diez',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>11</td><td>once</td><td><button class="speak-btn" onclick="speak('once',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>12</td><td>doce</td><td><button class="speak-btn" onclick="speak('doce',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>13</td><td>trece</td><td><button class="speak-btn" onclick="speak('trece',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>14</td><td>catorce</td><td><button class="speak-btn" onclick="speak('catorce',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>15</td><td>quince</td><td><button class="speak-btn" onclick="speak('quince',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>16</td><td>dieciséis</td><td><button class="speak-btn" onclick="speak('dieciséis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>17</td><td>diecisiete</td><td><button class="speak-btn" onclick="speak('diecisiete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>18</td><td>dieciocho</td><td><button class="speak-btn" onclick="speak('dieciocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>19</td><td>diecinueve</td><td><button class="speak-btn" onclick="speak('diecinueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>20</td><td>veinte</td><td><button class="speak-btn" onclick="speak('veinte',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>21</td><td>veintiuno</td><td><button class="speak-btn" onclick="speak('veintiuno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>22</td><td>veintidós</td><td><button class="speak-btn" onclick="speak('veintidós',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>23</td><td>veintitrés</td><td><button class="speak-btn" onclick="speak('veintitrés',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>24</td><td>veinticuatro</td><td><button class="speak-btn" onclick="speak('veinticuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>25</td><td>veinticinco</td><td><button class="speak-btn" onclick="speak('veinticinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>26</td><td>veintiséis</td><td><button class="speak-btn" onclick="speak('veintiséis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>27</td><td>veintisiete</td><td><button class="speak-btn" onclick="speak('veintisiete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>28</td><td>veintiocho</td><td><button class="speak-btn" onclick="speak('veintiocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>29</td><td>veintinueve</td><td><button class="speak-btn" onclick="speak('veintinueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>30</td><td>treinta</td><td><button class="speak-btn" onclick="speak('treinta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>31</td><td>treinta y uno</td><td><button class="speak-btn" onclick="speak('treinta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>32</td><td>treinta y dos</td><td><button class="speak-btn" onclick="speak('treinta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>33</td><td>treinta y tres</td><td><button class="speak-btn" onclick="speak('treinta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>34</td><td>treinta y cuatro</td><td><button class="speak-btn" onclick="speak('treinta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>35</td><td>treinta y cinco</td><td><button class="speak-btn" onclick="speak('treinta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>36</td><td>treinta y seis</td><td><button class="speak-btn" onclick="speak('treinta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>37</td><td>treinta y siete</td><td><button class="speak-btn" onclick="speak('treinta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>38</td><td>treinta y ocho</td><td><button class="speak-btn" onclick="speak('treinta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>39</td><td>treinta y nueve</td><td><button class="speak-btn" onclick="speak('treinta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>40</td><td>cuarenta</td><td><button class="speak-btn" onclick="speak('cuarenta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>41</td><td>cuarenta y uno</td><td><button class="speak-btn" onclick="speak('cuarenta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>42</td><td>cuarenta y dos</td><td><button class="speak-btn" onclick="speak('cuarenta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>43</td><td>cuarenta y tres</td><td><button class="speak-btn" onclick="speak('cuarenta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>44</td><td>cuarenta y cuatro</td><td><button class="speak-btn" onclick="speak('cuarenta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>45</td><td>cuarenta y cinco</td><td><button class="speak-btn" onclick="speak('cuarenta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>46</td><td>cuarenta y seis</td><td><button class="speak-btn" onclick="speak('cuarenta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>47</td><td>cuarenta y siete</td><td><button class="speak-btn" onclick="speak('cuarenta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>48</td><td>cuarenta y ocho</td><td><button class="speak-btn" onclick="speak('cuarenta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>49</td><td>cuarenta y nueve</td><td><button class="speak-btn" onclick="speak('cuarenta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>50</td><td>cincuenta</td><td><button class="speak-btn" onclick="speak('cincuenta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>51</td><td>cincuenta y uno</td><td><button class="speak-btn" onclick="speak('cincuenta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>52</td><td>cincuenta y dos</td><td><button class="speak-btn" onclick="speak('cincuenta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>53</td><td>cincuenta y tres</td><td><button class="speak-btn" onclick="speak('cincuenta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>54</td><td>cincuenta y cuatro</td><td><button class="speak-btn" onclick="speak('cincuenta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>55</td><td>cincuenta y cinco</td><td><button class="speak-btn" onclick="speak('cincuenta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>56</td><td>cincuenta y seis</td><td><button class="speak-btn" onclick="speak('cincuenta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>57</td><td>cincuenta y siete</td><td><button class="speak-btn" onclick="speak('cincuenta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>58</td><td>cincuenta y ocho</td><td><button class="speak-btn" onclick="speak('cincuenta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>59</td><td>cincuenta y nueve</td><td><button class="speak-btn" onclick="speak('cincuenta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>60</td><td>sesenta</td><td><button class="speak-btn" onclick="speak('sesenta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>61</td><td>sesenta y uno</td><td><button class="speak-btn" onclick="speak('sesenta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>62</td><td>sesenta y dos</td><td><button class="speak-btn" onclick="speak('sesenta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>63</td><td>sesenta y tres</td><td><button class="speak-btn" onclick="speak('sesenta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>64</td><td>sesenta y cuatro</td><td><button class="speak-btn" onclick="speak('sesenta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>65</td><td>sesenta y cinco</td><td><button class="speak-btn" onclick="speak('sesenta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>66</td><td>sesenta y seis</td><td><button class="speak-btn" onclick="speak('sesenta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>67</td><td>sesenta y siete</td><td><button class="speak-btn" onclick="speak('sesenta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>68</td><td>sesenta y ocho</td><td><button class="speak-btn" onclick="speak('sesenta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>69</td><td>sesenta y nueve</td><td><button class="speak-btn" onclick="speak('sesenta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>70</td><td>setenta</td><td><button class="speak-btn" onclick="speak('setenta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>71</td><td>setenta y uno</td><td><button class="speak-btn" onclick="speak('setenta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>72</td><td>setenta y dos</td><td><button class="speak-btn" onclick="speak('setenta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>73</td><td>setenta y tres</td><td><button class="speak-btn" onclick="speak('setenta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>74</td><td>setenta y cuatro</td><td><button class="speak-btn" onclick="speak('setenta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>75</td><td>setenta y cinco</td><td><button class="speak-btn" onclick="speak('setenta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>76</td><td>setenta y seis</td><td><button class="speak-btn" onclick="speak('setenta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>77</td><td>setenta y siete</td><td><button class="speak-btn" onclick="speak('setenta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>78</td><td>setenta y ocho</td><td><button class="speak-btn" onclick="speak('setenta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>79</td><td>setenta y nueve</td><td><button class="speak-btn" onclick="speak('setenta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>80</td><td>ochenta</td><td><button class="speak-btn" onclick="speak('ochenta',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>81</td><td>ochenta y uno</td><td><button class="speak-btn" onclick="speak('ochenta y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>82</td><td>ochenta y dos</td><td><button class="speak-btn" onclick="speak('ochenta y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>83</td><td>ochenta y tres</td><td><button class="speak-btn" onclick="speak('ochenta y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>84</td><td>ochenta y cuatro</td><td><button class="speak-btn" onclick="speak('ochenta y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>85</td><td>ochenta y cinco</td><td><button class="speak-btn" onclick="speak('ochenta y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>86</td><td>ochenta y seis</td><td><button class="speak-btn" onclick="speak('ochenta y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>87</td><td>ochenta y siete</td><td><button class="speak-btn" onclick="speak('ochenta y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>88</td><td>ochenta y ocho</td><td><button class="speak-btn" onclick="speak('ochenta y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>89</td><td>ochenta y nueve</td><td><button class="speak-btn" onclick="speak('ochenta y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>90</td><td>noventa</td><td><button class="speak-btn" onclick="speak('noventa',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>91</td><td>noventa y uno</td><td><button class="speak-btn" onclick="speak('noventa y uno',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>92</td><td>noventa y dos</td><td><button class="speak-btn" onclick="speak('noventa y dos',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>93</td><td>noventa y tres</td><td><button class="speak-btn" onclick="speak('noventa y tres',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>94</td><td>noventa y cuatro</td><td><button class="speak-btn" onclick="speak('noventa y cuatro',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>95</td><td>noventa y cinco</td><td><button class="speak-btn" onclick="speak('noventa y cinco',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>96</td><td>noventa y seis</td><td><button class="speak-btn" onclick="speak('noventa y seis',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>97</td><td>noventa y siete</td><td><button class="speak-btn" onclick="speak('noventa y siete',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>98</td><td>noventa y ocho</td><td><button class="speak-btn" onclick="speak('noventa y ocho',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>99</td><td>noventa y nueve</td><td><button class="speak-btn" onclick="speak('noventa y nueve',this)" title="Pronunciar">🔊</button></td></tr>
            <tr><td>100</td><td>cien</td><td><button class="speak-btn" onclick="speak('cien',this)" title="Pronunciar">🔊</button></td></tr>
          </tbody>
        </table>

        <!-- RANDOM -->
        <table id="random" class="word-table">
          <thead><tr><th>Inglés</th><th>Español</th><th></th></tr></thead>
          <tbody id="random-body"></tbody>
        </table>

        <a class="back" href="/Tools">&larr; Back to Tools</a>

        <script>
          const ALL = [
            ["house","la casa"],["dog","el perro"],["cat","el gato"],["water","el agua"],
            ["food","la comida"],["man","el hombre"],["woman","la mujer"],["city","la ciudad"],
            ["school","la escuela"],["book","el libro"],["car","el coche"],["day","el día"],
            ["night","la noche"],["family","la familia"],["friend","el amigo"],
            ["work","el trabajo"],["money","el dinero"],["time","el tiempo"],["door","la puerta"],
            ["table","la mesa"],["chair","la silla"],["window","la ventana"],["street","la calle"],
            ["to be","ser"],["to have","tener"],["to do","hacer"],["to go","ir"],
            ["to say","decir"],["to see","ver"],["to want","querer"],["to come","venir"],
            ["to know","saber"],["to speak","hablar"],["to eat","comer"],
            ["to drink","beber"],["to sleep","dormir"],["to work","trabajar"],["to live","vivir"],
            ["to think","pensar"],["to give","dar"],["to read","leer"],["to write","escribir"],
            ["to buy","comprar"],["to open","abrir"],["to close","cerrar"]
          ];

          // ---- Quick Translate ----
          let qtDir = 'en2es';

          function qtToggleDir() {
            qtDir = qtDir === 'en2es' ? 'es2en' : 'en2es';
            document.getElementById('qt-dir').textContent =
              qtDir === 'en2es' ? 'Inglés → Español' : 'Español → Inglés';
            document.getElementById('qt-output').classList.remove('visible');
            document.getElementById('qt-speak-out').style.display = 'none';
          }

          function qtCharCount() {
            const len = document.getElementById('qt-input').value.length;
            // subtle visual warning near limit
            document.getElementById('qt-input').style.borderColor = len > 90 ? '#c0392b' : '';
          }

          // Speak helpers — routes Spanish to Azure, English to browser TTS
          async function speakAny(text, lang, btn) {
            if (!text) return;
            if (currentAudio) { currentAudio.pause(); currentAudio = null; }
            document.querySelectorAll('.speak-btn').forEach(b => b.classList.remove('speaking'));

            if (lang === 'es') {
              // Azure Neural TTS
              btn.classList.add('speaking');
              try {
                const url = '/Tools/Spanish/tts?voice=' + selectedVoice + '&text=' + encodeURIComponent(text);
                const res = await fetch(url);
                if (!res.ok) { btn.classList.remove('speaking'); return; }
                const blob = await res.blob();
                const src  = URL.createObjectURL(blob);
                currentAudio = new Audio(src);
                currentAudio.onended = () => { btn.classList.remove('speaking'); URL.revokeObjectURL(src); };
                currentAudio.onerror = () => btn.classList.remove('speaking');
                currentAudio.play();
              } catch { btn.classList.remove('speaking'); }
            } else {
              // Browser TTS for English
              const utt = new SpeechSynthesisUtterance(text);
              utt.lang = 'en-US';
              btn.classList.add('speaking');
              utt.onend  = () => btn.classList.remove('speaking');
              utt.onerror = () => btn.classList.remove('speaking');
              speechSynthesis.cancel();
              speechSynthesis.speak(utt);
            }
          }

          function qtSpeakInput(btn) {
            const text = document.getElementById('qt-input').value.trim();
            const lang = qtDir === 'en2es' ? 'en' : 'es';
            speakAny(text, lang, btn);
          }

          function qtSpeakOutput(btn) {
            const text = document.getElementById('qt-output').textContent.trim();
            const lang = qtDir === 'en2es' ? 'es' : 'en';
            speakAny(text, lang, btn);
          }

          async function qtTranslate() {
            const text = document.getElementById('qt-input').value.trim();
            if (!text) return;
            const btn = document.getElementById('qt-btn');
            const out = document.getElementById('qt-output');
            btn.disabled = true;
            btn.textContent = '...';
            out.classList.remove('visible');

            try {
              const body = new URLSearchParams({ text, dir: qtDir });
              const res  = await fetch('/Tools/Spanish/translate', { method: 'POST', body });
              const result = await res.text();
              out.textContent = result;
              out.classList.add('visible');
              document.getElementById('qt-speak-out').style.display = 'inline-block';
            } catch(e) {
              out.textContent = 'Error al traducir.';
              out.classList.add('visible');
            } finally {
              btn.disabled = false;
              btn.textContent = 'Traducir';
            }
          }

          // ---- Voice selection ----
          let selectedVoice = 'f';

          function setVoice(v, btn) {
            selectedVoice = v;
            document.querySelectorAll('.voice-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
          }

          // ---- Azure Neural TTS ----
          let currentAudio = null;

          async function speak(text, btn) {
            if (currentAudio) { currentAudio.pause(); currentAudio = null; }
            document.querySelectorAll('.speak-btn').forEach(b => b.classList.remove('speaking'));

            btn.classList.add('speaking');
            try {
              const url = '/Tools/Spanish/tts?voice=' + selectedVoice + '&text=' + encodeURIComponent(text);
              const res = await fetch(url);
              if (!res.ok) { btn.classList.remove('speaking'); return; }
              const blob = await res.blob();
              const src  = URL.createObjectURL(blob);
              currentAudio = new Audio(src);
              currentAudio.onended = () => {
                btn.classList.remove('speaking');
                URL.revokeObjectURL(src);
              };
              currentAudio.onerror = () => btn.classList.remove('speaking');
              currentAudio.play();
            } catch(e) {
              btn.classList.remove('speaking');
            }
          }

          // ---- Tables ----
          function buildRandom() {
            const shuffled = [...ALL].sort(() => Math.random() - 0.5).slice(0, 20);
            document.getElementById('random-body').innerHTML = shuffled.map(([en, es]) =>
              `<tr><td>${en}</td><td>${es}</td><td><button class="speak-btn" onclick="speak('${es}',this)" title="Pronunciar">🔊</button></td></tr>`
            ).join('');
          }
          buildRandom();

          function show(id, btn) {
            document.querySelectorAll('.word-table').forEach(t => t.classList.remove('visible'));
            document.querySelectorAll('.btn').forEach(b => b.classList.remove('active'));
            if (id === 'random') buildRandom();
            const table = document.getElementById(id);
            table.classList.add('visible');
            btn.classList.add('active');
            document.getElementById('count-label').textContent =
              table.querySelectorAll('tbody tr').length + ' palabras';
          }
        </script>
        """;
}
