using WebFrame;

namespace GTSample.Pages;

/// <summary>
/// Demonstrates <see cref="StaticPage"/>: serves a fixed HTML article.
/// </summary>
public class StaticPageSample : StaticPage
{
    public override string Title => "Static Page Sample";
    public override string Route => "/Sample/static";

    protected override string HtmlContent => """
        <style>
          article { background:#fff; border:1px solid #dde3ea; border-radius:8px; padding:2rem; }
          article h2 { margin-top:0; color:#1a3c5e; }
          blockquote { border-left:4px solid #1a3c5e; margin:1rem 0; padding:.5rem 1rem; background:#f0f4f8; color:#444; }
          .tag { display:inline-block; background:#1a3c5e; color:#fff; border-radius:4px; padding:.15rem .5rem; font-size:.75rem; margin:.15rem; }
          a.back { display:inline-block; margin-top:1.5rem; color:#1a3c5e; }
        </style>

        <article>
          <h2>The Marvels of the Mantis Shrimp</h2>

          <p>
            The <strong>mantis shrimp</strong> (<em>Stomatopoda</em>) is one of the ocean's most
            extraordinary creatures — and one of its most ruthless hunters.
          </p>

          <h3>Sixteen Colour Receptors</h3>
          <p>
            While humans perceive colour through three photoreceptor types (red, green, blue),
            mantis shrimp possess <strong>up to sixteen</strong>. Despite this, research suggests
            they do <em>not</em> see more colours than we do — instead they identify colour
            instantaneously without needing to compare wavelengths, much like a barcode scanner.
          </p>

          <h3>The Fastest Punch in the Sea</h3>
          <p>
            The peacock mantis shrimp accelerates its club-like appendage at
            <strong>10,000&nbsp;g</strong>, reaching speeds of up to
            <strong>80&nbsp;km/h</strong> — fast enough to shatter aquarium glass and create
            cavitation bubbles that generate a secondary shockwave even if the punch misses.
          </p>

          <blockquote>
            "If humans could punch at the same <em>relative</em> force as a mantis shrimp,
            we could break the sound barrier with our fists."
          </blockquote>

          <h3>Built-in Polarised Light Detection</h3>
          <p>
            Their eyes can detect <strong>circular polarised light</strong> — a capability no
            other animal is known to possess. They use this to communicate secretly with
            members of their own species, invisible to predators that lack the receptor.
          </p>

          <h3>Key Facts</h3>
          <ul>
            <li>Lifespan: 3–6 years in the wild</li>
            <li>Habitat: Tropical and subtropical seas worldwide</li>
            <li>Size: 5–38 cm depending on species</li>
            <li>Diet: Fish, crabs, molluscs, and other crustaceans</li>
          </ul>

          <div>
            <span class="tag">Marine Biology</span>
            <span class="tag">Optics</span>
            <span class="tag">Biomechanics</span>
            <span class="tag">Evolution</span>
          </div>

          <a class="back" href="/Sample">&larr; Back to Home</a>
        </article>
        """;
}
